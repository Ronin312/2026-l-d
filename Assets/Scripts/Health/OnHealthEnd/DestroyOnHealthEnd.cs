using UnityEngine;

namespace FrostGame.Health
{
    [RequireComponent(typeof(Health))]
    public class DestroyOnHealthEnd : MonoBehaviour
    {
      void Start()
        {
            GetComponent<Health>().OnDied += RemoveThis;
        }

        void RemoveThis(Health health)
        {
            Destroy(gameObject);
        }
    }
}
