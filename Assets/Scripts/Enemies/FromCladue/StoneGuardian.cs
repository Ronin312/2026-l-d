using UnityEngine;
using FrostGame.Health;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Kamienny Strażnik: wolny tank. Maszyna stanów: Patrol → Chase → Windup → Recovery,
    /// a po rozbiciu pancerza Stunned, po którym przez chwilę jest szybszy i podatniejszy na obrażenia.
    /// </summary>
    public class StoneGuardian : EnemyBase
    {
        enum State { Patrol, Chase, Windup, Recovery, Stunned }

        [SerializeField] StoneGuardianEnemyData stoneGuardianEnemyData;
        [Header("Patrol")]
        [SerializeField] Transform[] patrolPoints;
        [SerializeField] float patrolWait = 1f;

        [Header("Zamach")]
        [SerializeField] float slamOffset = 1.2f;
        [SerializeField] float recoveryTime = 0.6f;
        [SerializeField] LayerMask playerLayer;
        [SerializeField] DamageZone groundMarkPrefab;

        [Header("Pancerz")]
        [SerializeField] float exposedDuration = 4f;
        [SerializeField] float exposedDamageMultiplier = 2f;
        [SerializeField] float exposedSpeedMultiplier = 1.8f;

        [Header("Kolory (feedback)")]
        [SerializeField] Color windupColor = new Color(1f, 0.55f, 0.3f);
        [SerializeField] Color stunColor = new Color(1f, 1f, 0.4f);
        [SerializeField] Color exposedColor = new Color(1f, 0.75f, 0.75f);

        State state = State.Patrol;
        float stateEnd;
        float nextSlamTime;
        float waitUntil;
        float exposedEnd;
        int armor;
        int patrolIndex;
        bool exposed;
        Vector2 slamDir;

        bool Vulnerable => state == State.Stunned || exposed;
        float CurrentSpeed => data.moveSpeed * (exposed ? exposedSpeedMultiplier : 1f);

        protected override void Awake()
        {
            base.Awake();
            armor = stoneGuardianEnemyData.armorHealth;
        }

        protected override void Tick()
        {
            if (exposed && Time.time >= exposedEnd)
            {
                exposed = false;
                armor = stoneGuardianEnemyData.armorHealth; // pancerz odrasta po fazie osłabienia
                RefreshTint();
            }

            switch (state)
            {
                case State.Patrol: TickPatrol(); break;
                case State.Chase: TickChase(); break;
                case State.Windup:
                    if (Time.time >= stateEnd) Slam();
                    break;
                case State.Recovery:
                    if (Time.time >= stateEnd) state = State.Chase;
                    break;
                case State.Stunned:
                    if (Time.time >= stateEnd) EndStun();
                    break;
            }
        }

        void TickPatrol()
        {
            if (target != null && DistanceToTarget <= data.detectionRange)
            {
                state = State.Chase;
                return;
            }

            if (patrolPoints == null || patrolPoints.Length == 0 || Time.time < waitUntil) return;

            Vector2 point = patrolPoints[patrolIndex].position;
            if (Vector2.Distance(rb.position, point) < 0.15f)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                waitUntil = Time.time + patrolWait;
            }
            else
            {
                Move(point - rb.position, data.moveSpeed * 0.5f);
            }
        }

        void TickChase()
        {
            if (target == null || DistanceToTarget > data.detectionRange * 1.5f)
            {
                state = State.Patrol;
                return;
            }

            if (DistanceToTarget <= stoneGuardianEnemyData.attackRange && Time.time >= nextSlamTime)
            {
                state = State.Windup;
                stateEnd = Time.time + stoneGuardianEnemyData.windupTime;
                slamDir = DirectionToTarget; // kierunek zamachu blokuje się na starcie zapowiedzi
                SetTint(windupColor);
                return;
            }

            Move(DirectionToTarget, CurrentSpeed);
        }

        void Slam()
        {
            Vector2 center = rb.position + slamDir * slamOffset;

            var hits = Physics2D.OverlapCircleAll(center, stoneGuardianEnemyData.slamRadius, playerLayer);
            var damaged = new System.Collections.Generic.HashSet<IDamageable>();
            foreach (var hit in hits)
            {
                var damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable != null && damaged.Add(damageable))
                    damageable.TakeDamage(stoneGuardianEnemyData.slamDamage);
            }

            if (groundMarkPrefab != null)
                Instantiate(groundMarkPrefab, center, Quaternion.identity);

            nextSlamTime = Time.time + stoneGuardianEnemyData.slamCooldown;
            state = State.Recovery;
            stateEnd = Time.time + recoveryTime;
            RefreshTint();
        }

        public override void TakeDamage(int amount)
        {
            if (isDead) return;
            if (TryAbsorb()) return; // tarcza Szamana ma pierwszeństwo przed pancerzem

            if (Vulnerable)
            {
                base.TakeDamage(Mathf.RoundToInt(amount * exposedDamageMultiplier));
                return;
            }

            if (armor > 0)
            {
                armor -= amount;
                Flash();
                if (armor <= 0) BreakArmor();
                return;
            }

            base.TakeDamage(amount);
        }

        void BreakArmor()
        {
            state = State.Stunned;
            stateEnd = Time.time + stoneGuardianEnemyData.stunDuration;
            SetTint(stunColor);
        }

        void EndStun()
        {
            state = State.Chase;
            exposed = true;
            exposedEnd = Time.time + exposedDuration;
            RefreshTint();
        }

        void RefreshTint() => SetTint(exposed ? exposedColor : Color.white);

        void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stoneGuardianEnemyData.attackRange);
        }
    }
}