using UnityEngine;

namespace FrostGame.Bullet
{
    /// <summary>
    /// Pocisk wystrzeliwany przez gracza (lub wrogów). Wymaga Rigidbody2D (Kinematic) i Collider2D (Is Trigger).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 12f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 3f;
        //[SerializeField] private LayerMask hitMask = ~0;
        //[SerializeField] private LayerMask ignoreMask; // np. warstwa strzelającego

        private Rigidbody2D rb;

        public void Launch(Vector2 direction, float damageOverride = -1f)
        {
            if (damageOverride >= 0f) damage = damageOverride;

            rb = GetComponent<Rigidbody2D>();
            //rb.gravityScale = 0f;
            rb.linearVelocity = direction.normalized * speed;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Destroy(gameObject, lifetime);
        }
        //}

        //private void OnTriggerEnter2D(Collider2D other)
        //{
        //    if (other.isTrigger) return; // portale, grzyby, NPC nie zatrzymują pocisku

        //    int layerBit = 1 << other.gameObject.layer;
        //    if ((ignoreMask & layerBit) != 0) return;
        //    if ((hitMask & layerBit) == 0) return;

        //    //if (other.TryGetComponent(out Core.Health health))
        //    //    health.TakeDamage(damage);

        //    Destroy(gameObject);
        //}

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log("kolizja pocisku z " + collision.gameObject.name);
            if (collision.gameObject.TryGetComponent(out Health.Health health))
            {
                health.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        //private void OnCollision(Collision collision)
        //{
        //    Debug.Log("kolizja pocisku z " + collision.gameObject.name);
        //    if (collision.gameObject.TryGetComponent(out Health.Health health))
        //    {
        //        health.TakeDamage(damage);
        //    }
        //    Destroy(gameObject);
        //}
    }
}
