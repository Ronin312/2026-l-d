using UnityEngine;
using FrostGame.Health;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Pocisk Łucznika. Wymaga Collider2D (Is Trigger) i Rigidbody2D (ustawiany na Kinematic).
    /// Rani gracza (tag "Player"), znika na przeszkodach.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] float lifetime = 4f;

        Vector2 direction;
        float speed;
        int damage;

        void Awake()
        {
            var body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
            GetComponent<Collider2D>().isTrigger = true;
        }

        public void Init(Vector2 dir, int dmg, float spd)
        {
            direction = dir.normalized;
            damage = dmg;
            speed = spd;
            transform.right = direction;
            Destroy(gameObject, lifetime);
        }

        void Update()
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<EnemyBase>() != null) return; // ignoruj wrogów
            if (other.isTrigger) return;

            if (other.CompareTag("Player"))
            {
                var damageable = other.GetComponentInParent<IDamageable>();
                if (damageable != null) damageable.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
