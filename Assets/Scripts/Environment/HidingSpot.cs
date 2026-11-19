using System.Collections;
using UnityEngine;
using FrostGame.Health;

namespace FrostGame.Environment
{
    public class HidingSpot : MonoBehaviour, IDamageable
    {
        [Header("Rustle / Shudder Effect")]
        [SerializeField] Animator spotAnimator;
        [SerializeField] float minRustleInterval = 2.0f;
        [SerializeField] float maxRustleInterval = 4.5f;

        [Header("Health / Durability")]
        [SerializeField] int health = 1;

        public bool IsOccupied { get; private set; }
        public bool IsDestroyed { get; private set; }

        public event System.Action OnSpotDestroyed;

        Coroutine rustleRoutine;

        public void SetOccupied(bool occupied)
        {
            if (IsDestroyed) return;
            IsOccupied = occupied;

            if (occupied)
            {
                if (rustleRoutine == null)
                    rustleRoutine = StartCoroutine(RustleLoop());
            }
            else
            {
                if (rustleRoutine != null)
                {
                    StopCoroutine(rustleRoutine);
                    rustleRoutine = null;
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (IsDestroyed) return;

            health -= damage;
            if (health <= 0)
            {
                DestroySpot();
            }
        }

        public void DestroySpot()
        {
            if (IsDestroyed) return;

            IsDestroyed = true;
            IsOccupied = false;

            if (rustleRoutine != null)
            {
                StopCoroutine(rustleRoutine);
                rustleRoutine = null;
            }
            //notify archer inside
            OnSpotDestroyed?.Invoke();

            gameObject.SetActive(false);
        }

        IEnumerator RustleLoop()
        {
            while (IsOccupied && !IsDestroyed)
            {
                yield return new WaitForSeconds(Random.Range(minRustleInterval, maxRustleInterval));
                if (spotAnimator != null)
                {
                    spotAnimator.SetTrigger("Rustle");
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.4f);
            Gizmos.DrawWireCube(transform.position, new Vector3(1.2f, 1.2f, 0));
        }
    }
}
