using FrostGame.UI;
using UnityEngine;

namespace FrostGame.Rebind
{
    [RequireComponent(typeof(UnityEngine.UI.Button))]
    public class ResetBindButton : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                SaveLoadBindings.Instance.ResetToDefault();
                SimpleRebindButton[] rebindButtons = FindObjectsByType<SimpleRebindButton>();
                foreach (SimpleRebindButton button in rebindButtons)
                {
                    button.UpdateUI();
                }
            });
        }

    }
}
