
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace FrostGame.PlayerStuf
{

    [RequireComponent(typeof(PlayerControl))]
    public class PlayerExit : MonoBehaviour
    {
        [SerializeField]string mainMenuSceneName = "MainMenu";
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GetComponent<PlayerControl>().OnExitAction += ExitGame;
        }

        private void ExitGame()
        {
           SceneManager.LoadScene(mainMenuSceneName);
        }

    }
}
