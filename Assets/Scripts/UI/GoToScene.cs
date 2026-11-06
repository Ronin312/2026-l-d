using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace FrostGame.UI
{
    [RequireComponent(typeof(Button))]
    public class GoToScene : MonoBehaviour
    {
        [SerializeField]
        private string sceneName;

      void Start()
        {
            GetComponent<Button>().onClick.AddListener(LoadScene);// () => SceneManager.LoadScene(sceneName));
        }

        void LoadScene()
        {
            if(string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("Scene name is not set.");
                return;
            }
            SceneManager.LoadScene(sceneName);
        }
    }
}
