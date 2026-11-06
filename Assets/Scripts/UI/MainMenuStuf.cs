using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

namespace FrostGame.UI
{
    public class MainMenuStuf : MonoBehaviour
    {
        [SerializeField] InputSystemUIInputModule inputModule;
        [SerializeField] private Button playBtn;
        [SerializeField] private string gameSceneToLoad;
        [SerializeField] private Button optionsBtn;
        [SerializeField] private string optionsSceneToLoad;
        [SerializeField] private Button exitBtn;

        [SerializeField] private TextMeshProUGUI titleText;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            playBtn.onClick.AddListener(OnPlayButtonClicked);
            optionsBtn.onClick.AddListener(OnOptionsButtonClicked);
            exitBtn.onClick.AddListener(OnExitButtonClicked);

            titleText.text = Application.productName;
            PlayBtnFocus();

        }

        private void OnEnable()
        {
            inputModule.cancel.action.performed += OnCancelPerformed;
        }

        private void OnDisable()
        {
            inputModule.cancel.action.performed -= OnCancelPerformed;
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            StartCoroutine(QuitGameRoutine());
        }

        private System.Collections.IEnumerator QuitGameRoutine()
        {
            // Czekamy na koniec bieżącej klatki, aż Input System skończy przetwarzać kliknięcie
            yield return new WaitForEndOfFrame();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }

        void PlayBtnFocus()
        {     
            playBtn.Select();
        }

        private void OnExitButtonClicked()
        {
            StartCoroutine(QuitGameRoutine());

            //#if UNITY_EDITOR

            //            UnityEditor.EditorApplication.isPlaying = false;
            //#else
            //            Application.Quit();
            //#endif
        }

        private void OnOptionsButtonClicked()
        {
            inputModule.enabled = false;
            SceneManager.LoadScene(optionsSceneToLoad);
            PlayBtnFocus();
        }

        private void OnPlayButtonClicked()
        {
            inputModule.enabled = false;
            SceneManager.LoadScene(gameSceneToLoad);
            PlayBtnFocus();
        }

    }
}