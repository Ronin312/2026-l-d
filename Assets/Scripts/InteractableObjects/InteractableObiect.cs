using UnityEngine;

namespace FrostGame.InteractableObjects
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractableObiect : MonoBehaviour
    {
        protected Collider2D col;
        private void Start()
        {
            col = GetComponent<Collider2D>();
        }

        public virtual bool Interact()
        {
            Debug.Log("Interacting with " + gameObject.name);
            // Disable the collider to prevent further interactions
            col.contactCaptureLayers = 0;
            col.callbackLayers = 0;
            col.enabled = false;
          
            Destroy(gameObject);
            return true;
        }
    }
}
