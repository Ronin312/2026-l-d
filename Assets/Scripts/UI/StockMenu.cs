using FrostGame.PlayerStuf;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FrostGame.UI
{
    public class StockMenu : MonoBehaviour
    {
        [SerializeField] PlayerControl pc;

        [SerializeField] Button backBtn;
        [SerializeField] MonoBehaviour[] monoToDisactivate;
        [SerializeField] GameObject[] gobToDisactivate;

        private void Awake()
        {
            pc.OnStockAction += Pc_OnStockAction;
            backBtn.onClick.AddListener(Pc_OnStockAction);
            Pc_OnStockAction();
        }

        private void Pc_OnStockAction()
        {
           if(gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                gameObject.SetActive(true);
                Time.timeScale = 0;
            }

           foreach(GameObject g in gobToDisactivate)
            {
                g.SetActive(!gameObject.activeSelf);
            }

            foreach (MonoBehaviour m in monoToDisactivate)
            {
                m.enabled = (!gameObject.activeSelf);
            }
        }
    }
}