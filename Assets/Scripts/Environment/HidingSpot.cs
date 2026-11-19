using System.Collections;
using UnityEngine;

namespace FrostGame.Environment
{
    public class HidingSpot : MonoBehaviour
    {
        [Header("Rustle/Shudder Effect")]
        [SerializeField] Animator spotAnimator; 
        [SerializeField] float minRustleInterval = 2.5f; 
        [SerializeField] float maxRustleInterval = 4.5f;
        
        public bool IsOccupied { get; private set; }
        public bool IsDestroyed { get; private set; }
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

        public void DestroySpot()
        {
            IsDestroyed = true;
            IsOccupied = false;
            if (rustleRoutine == null) StopCoroutine(rustleRoutine);
            gameObject.SetActive(false);
        }
        
        IEnumerator RustleLoop()
        {
            while (IsOccupied)
            {
                yield return new WaitForSeconds(Random.Range(minRustleInterval, maxRustleInterval));
                if (spotAnimator != null)
                {
                    spotAnimator.SetTrigger("Rustle");
                }
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.4f);
            Gizmos.DrawCube(transform.localPosition, new Vector3(1.2f, 1.2f, 0));
        }
    }
}


