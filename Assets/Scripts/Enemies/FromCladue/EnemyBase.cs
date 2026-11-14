using System;
using System.Collections;
using UnityEngine;
using FrostGame.Health;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Bazowa klasa wroga: HP, śmierć, obrażenia od kontaktu, ruch i znajdowanie gracza.
    /// Wymaga Rigidbody2D (Dynamic, Gravity Scale 0). Gracz musi mieć tag "Player".
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [SerializeField] protected EnemyData data;

        protected Rigidbody2D rb;
        protected SpriteRenderer sprite;
        protected Transform target;
        protected int currentHealth;
        protected bool isDead;

        Vector2 desiredVelocity;
        float nextContactTime;
        Color tint = Color.white;

        public event Action<EnemyBase> Died;

        public bool IsDead => isDead;

        protected float DistanceToTarget =>
            target != null ? Vector2.Distance(rb.position, target.position) : float.MaxValue;

        protected Vector2 DirectionToTarget =>
            target != null ? ((Vector2)target.position - rb.position).normalized : Vector2.zero;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sprite = GetComponentInChildren<SpriteRenderer>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            currentHealth = data.maxHealth;
        }

        protected virtual void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        void Update()
        {
            if (isDead) return;
            desiredVelocity = Vector2.zero;
            Tick();
        }

        void FixedUpdate()
        {
            if (isDead || desiredVelocity == Vector2.zero) return;
            rb.MovePosition(rb.position + desiredVelocity * Time.fixedDeltaTime);
        }

        /// <summary>Logika wroga, wywoływana co klatkę. Ruch zleca się przez Move().</summary>
        protected abstract void Tick();

        protected void Move(Vector2 direction, float speed)
        {
            desiredVelocity = direction.normalized * speed;
        }

        /// <summary>Sprawdza, czy tarcza (ShieldBuff) pochłonęła trafienie.</summary>
        protected bool TryAbsorb()
        {
            if (TryGetComponent(out ShieldBuff shield) && shield.Absorb())
            {
                Flash();
                return true;
            }
            return false;
        }

        public virtual void TakeDamage(int amount)
        {
            if (isDead) return;
            if (TryAbsorb()) return;
            currentHealth -= amount;
            Flash();
            if (currentHealth <= 0) Die();
        }

        protected virtual void Die()
        {
            if (isDead) return;
            isDead = true;
            Died?.Invoke(this);
            Destroy(gameObject);
        }

        protected void SetTint(Color color)
        {
            tint = color;
            if (sprite != null) sprite.color = color;
        }

        protected void Flash()
        {
            if (sprite != null && gameObject.activeInHierarchy) StartCoroutine(FlashRoutine());
        }

        IEnumerator FlashRoutine()
        {
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.08f);
            if (sprite != null) sprite.color = tint;
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if (isDead || data.contactDamage <= 0 || Time.time < nextContactTime) return;
            if (!collision.collider.CompareTag("Player")) return;

            var damageable = collision.collider.GetComponentInParent<IDamageable>();
            if (damageable == null) return;

            damageable.TakeDamage(data.contactDamage);
            nextContactTime = Time.time + data.contactCooldown;
        }
    }
}
