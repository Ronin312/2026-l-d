using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FrostGame.UI
{
    public class MainMenuStuf : MonoBehaviour
    {
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

        void PlayBtnFocus()
        {     
            playBtn.Select();
        }

        private void OnExitButtonClicked()
        {
            Application.Quit();      
        }

        private void OnOptionsButtonClicked()
        {
            SceneManager.LoadScene(optionsSceneToLoad);
            PlayBtnFocus();
        }

        private void OnPlayButtonClicked()
        {
            SceneManager.LoadScene(gameSceneToLoad);
            PlayBtnFocus();
        }

    }
}