using UnityEngine;
using FrostGame.Health;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Kret Zasadzkowiec: pod ziemią jest nietykalny i niewidoczny (widać tylko kopiec), podąża za graczem,
    /// zatrzymuje się pod nim, zapowiada wynurzenie okręgiem na ziemi, po czym wybucha obszarowo
    /// i na kilka sekund zostaje na powierzchni, gdzie można go bić.
    /// </summary>
    public class BurrowMole : EnemyBase
    {
        enum State { Underground, Telegraph, Surfaced }

        [SerializeField] BurrowMoleEnemyData burrowMoleEnemyData;
        [Header("Referencje")]
        [Tooltip("Collider ciała. Wyłączany pod ziemią. Jeśli puste, szukany na obiekcie.")]
        [SerializeField] Collider2D bodyCollider;
        [Tooltip("Kopiec / pęknięcia widoczne tylko pod ziemią (opcjonalnie).")]
        [SerializeField] GameObject moundEffect;
        [Tooltip("Okrąg zapowiedzi (sprite o rozmiarze 1 jednostki, skalowany do promienia wybuchu).")]
        [SerializeField] Transform telegraphMarker;
        [SerializeField] LayerMask playerLayer;
        [SerializeField] GameObject burstVfx;

        State state;
        float stateEnd;
        float undergroundEnd;

        protected override void Awake()
        {
            base.Awake();
            if (bodyCollider == null) bodyCollider = GetComponent<Collider2D>();
        }

        protected override void Start()
        {
            base.Start();
            GoUnderground();
        }

        protected override void Tick()
        {
            switch (state)
            {
                case State.Underground: TickUnderground(); break;
                case State.Telegraph:
                    if (Time.time >= stateEnd) Emerge();
                    break;
                case State.Surfaced:
                    if (Time.time >= stateEnd) GoUnderground();
                    break;
            }
        }

        void TickUnderground()
        {
            if (target == null) return;

            if (DistanceToTarget > data.detectionRange)
            {
                undergroundEnd = Time.time + burrowMoleEnemyData.moleUndergroundMaxTime; // brak gracza w zasięgu: czeka
                return;
            }

            if (DistanceToTarget <= burrowMoleEnemyData.moleTriggerDistance || Time.time >= undergroundEnd)
            {
                BeginTelegraph();
                return;
            }

            Move(DirectionToTarget, data.moveSpeed);
        }

        void BeginTelegraph()
        {
            state = State.Telegraph;
            stateEnd = Time.time + burrowMoleEnemyData.moleTelegraphTime;

            if (telegraphMarker != null)
            {
                telegraphMarker.gameObject.SetActive(true);
                telegraphMarker.localScale = Vector3.one * burrowMoleEnemyData.moleBurstRadius * 2f;
            }
        }

        void Emerge()
        {
            if (telegraphMarker != null) telegraphMarker.gameObject.SetActive(false);

            var hits = Physics2D.OverlapCircleAll(rb.position, burrowMoleEnemyData.moleBurstRadius, playerLayer);
            var damaged = new System.Collections.Generic.HashSet<IDamageable>();
            foreach (var hit in hits)
            {
                var damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable != null && damaged.Add(damageable))
                    damageable.TakeDamage(burrowMoleEnemyData.moleBurstDamage);
            }

            if (burstVfx != null) Instantiate(burstVfx, transform.position, Quaternion.identity);

            SetSurfaced(true);
            state = State.Surfaced;
            stateEnd = Time.time + burrowMoleEnemyData.moleSurfacedTime;
        }

        void GoUnderground()
        {
            SetSurfaced(false);
            state = State.Underground;
            undergroundEnd = Time.time + burrowMoleEnemyData.moleUndergroundMaxTime;
        }

        void SetSurfaced(bool surfaced)
        {
            if (sprite != null) sprite.enabled = surfaced;
            if (bodyCollider != null) bodyCollider.enabled = surfaced;
            if (moundEffect != null) moundEffect.SetActive(!surfaced);
        }

        public override void TakeDamage(int amount)
        {
            if (state != State.Surfaced) return; // pod ziemią nietykalny
            base.TakeDamage(amount);
        }

        void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, burrowMoleEnemyData.moleBurstRadius);
        }
    }
}
