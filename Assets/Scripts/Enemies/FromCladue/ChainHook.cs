using UnityEngine;
using FrostGame.Health;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Hak wyrzucany przez Łowcę z Łańcuchem. Leci w prostej linii; po trafieniu gracza przyciąga go
    /// do łowcy, po trafieniu przeszkody wraca i ogłusza łowcę. Można go przeciąć atakiem (IDamageable).
    /// Wymaga Collider2D (ustawiany jako Trigger) i Rigidbody2D (ustawiany jako Kinematic, bo bez niego
    /// trigger nie wykryje statycznych ścian). LineRenderer (2 punkty, World Space) rysuje łańcuch.
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class ChainHook : MonoBehaviour, IDamageable
    {
        [SerializeField] LineRenderer chain;

        ChainHunter owner;
        Vector2 direction;
        Vector2 startPosition;
        float speed;
        float maxRange;
        int damage;
        float pullSpeed;
        float pullDuration;
        float pullStopDistance;

        bool attached;
        bool ended;
        Transform attachedTo;
        IPullable pulled;
        float releaseTime;

        void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;

            var body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
        }

        public void Init(ChainHunter hunter, Vector2 dir, float hookSpeed, float range, int hookDamage,
                         float pullSpd, float pullDur, float stopDistance)
        {
            owner = hunter;
            direction = dir.normalized;
            speed = hookSpeed;
            maxRange = range;
            damage = hookDamage;
            pullSpeed = pullSpd;
            pullDuration = pullDur;
            pullStopDistance = stopDistance;
            startPosition = transform.position;
            transform.right = direction;
        }

        void Update()
        {
            if (ended) return;

            if (owner == null)
            {
                End(false, true);
                return;
            }

            if (!attached)
            {
                transform.position += (Vector3)(direction * speed * Time.deltaTime);
                if (Vector2.Distance(startPosition, transform.position) >= maxRange)
                    End(false, false);
            }
            else
            {
                if (attachedTo != null) transform.position = attachedTo.position;
                if (Time.time >= releaseTime) End(false, false);
            }
        }

        void LateUpdate()
        {
            if (chain == null || owner == null) return;

            chain.positionCount = 2;
            chain.SetPosition(0, owner.transform.position);
            chain.SetPosition(1, transform.position);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (ended || attached) return;
            if (other.GetComponentInParent<EnemyBase>() != null) return; // ignoruj wrogów
            if (other.isTrigger) return;

            if (other.CompareTag("Player"))
            {
                var damageable = other.GetComponentInParent<IDamageable>();
                if (damageable != null) damageable.TakeDamage(damage);

                pulled = other.GetComponentInParent<IPullable>();
                if (pulled == null)
                {
                    End(false, false);
                    return;
                }

                pulled.ApplyPull(owner.transform, pullSpeed, pullDuration, pullStopDistance);
                attached = true;
                attachedTo = other.transform;
                releaseTime = Time.time + pullDuration;
                return;
            }

            End(true, false); // przeszkoda
        }

        /// <summary>Przecięcie łańcucha atakiem gracza.</summary>
        public void TakeDamage(int amount)
        {
            End(false, true);
        }

        void End(bool blocked, bool cut)
        {
            if (ended) return;
            ended = true;

            if (cut && pulled != null) pulled.CancelPull();
            if (owner != null) owner.OnHookEnded(blocked);

            Destroy(gameObject);
        }
    }
}
