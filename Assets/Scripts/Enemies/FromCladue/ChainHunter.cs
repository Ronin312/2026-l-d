using System.Collections;
using UnityEngine;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Łowca z Łańcuchem: utrzymuje dystans, zapowiada rzut (cienka linia), po czym wyrzuca hak (ChainHook).
    /// Hak trafiony w gracza przyciąga go do łowcy. Hak można przeciąć atakiem.
    /// Jeśli hak trafi w przeszkodę, łowca jest ogłuszony i przyjmuje zwiększone obrażenia.
    /// Gracz musi mieć komponent IPullable (np. PlayerPullReceiver), żeby dało się go przyciągnąć.
    /// </summary>
    public class ChainHunter : EnemyBase
    {
        [SerializeField] ChainHunterEnemyData chainHunterEnemyData;
        [Header("Hak")]
        [SerializeField] ChainHook hookPrefab;
        [SerializeField] Transform firePoint;
        [SerializeField] LineRenderer aimLine;
        [SerializeField] LayerMask obstacleMask;
        [SerializeField] float aimLockTime = 0.2f;

        [Header("Kolory")]
        [SerializeField] Color stunColor = new Color(1f, 1f, 0.4f);

        bool busy;          // zapowiedź lub hak w locie / przyciąganie
        bool stunned;
        float nextHookTime;
        float stunEnd;
        float busyDeadline; // zabezpieczenie, gdyby hak zniknął bez wywołania OnHookEnded

        protected override void Start()
        {
            base.Start();
            if (firePoint == null) firePoint = transform;
            if (aimLine != null) aimLine.enabled = false;
            nextHookTime = Time.time + 1f;
        }

        protected override void Tick()
        {
            if (stunned && Time.time >= stunEnd)
            {
                stunned = false;
                SetTint(Color.white);
            }

            if (busy && Time.time > busyDeadline) busy = false;
            if (stunned || busy || target == null) return;

            float dist = DistanceToTarget;
            if (dist > data.detectionRange) return;

            if (dist < chainHunterEnemyData.hookKeepDistance - 1f)
                Move(-DirectionToTarget, data.moveSpeed);
            else if (dist > chainHunterEnemyData.hookKeepDistance + 1f)
                Move(DirectionToTarget, data.moveSpeed * 0.7f);

            if (dist <= chainHunterEnemyData.hookRange && Time.time >= nextHookTime && HasLineOfSight() && hookPrefab != null)
                StartCoroutine(TelegraphAndFire());
        }

        bool HasLineOfSight()
        {
            return !Physics2D.Linecast(firePoint.position, target.position, obstacleMask);
        }

        IEnumerator TelegraphAndFire()
        {
            busy = true;
            busyDeadline = Time.time + chainHunterEnemyData.hookTelegraphTime + 5f;

            if (aimLine != null)
            {
                aimLine.enabled = true;
                aimLine.positionCount = 2;
            }

            Vector2 dir = DirectionToTarget;
            float t = 0f;
            while (t < chainHunterEnemyData.hookTelegraphTime)
            {
                if (target == null) break;

                Vector2 origin = firePoint.position;
                if (t < chainHunterEnemyData.hookTelegraphTime - aimLockTime)
                    dir = ((Vector2)target.position - origin).normalized;

                if (aimLine != null)
                {
                    aimLine.SetPosition(0, origin);
                    aimLine.SetPosition(1, origin + dir * chainHunterEnemyData.hookRange);
                }

                t += Time.deltaTime;
                yield return null;
            }

            if (aimLine != null) aimLine.enabled = false;

            if (target == null)
            {
                busy = false;
                yield break;
            }

            var hook = Instantiate(hookPrefab, firePoint.position, Quaternion.identity);
            hook.Init(this, dir, chainHunterEnemyData.hookSpeed, chainHunterEnemyData.hookRange, chainHunterEnemyData.hookDamage,
                      chainHunterEnemyData.hookPullSpeed, chainHunterEnemyData.hookPullDuration, chainHunterEnemyData.hookStopDistance);

            // Czas lotu + przyciąganie + zapas; busy zdejmuje OnHookEnded.
            busyDeadline = Time.time + chainHunterEnemyData.hookRange / Mathf.Max(chainHunterEnemyData.hookSpeed, 0.1f) + chainHunterEnemyData.hookPullDuration + 2f;
        }

        /// <summary>Wołane przez ChainHook, gdy hak się zakończył (trafił, chybił, został przecięty).</summary>
        public void OnHookEnded(bool blocked)
        {
            busy = false;
            nextHookTime = Time.time + chainHunterEnemyData.hookCooldown;

            if (blocked && !isDead)
            {
                stunned = true;
                stunEnd = Time.time + chainHunterEnemyData.hookBlockedStunTime;
                SetTint(stunColor);
            }
        }

        public override void TakeDamage(int amount)
        {
            if (isDead) return;

            if (stunned)
                amount = Mathf.RoundToInt(amount * chainHunterEnemyData.hookStunDamageMultiplier);

            base.TakeDamage(amount);
        }

        void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, chainHunterEnemyData.hookRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, chainHunterEnemyData.hookKeepDistance);
        }
    }
}
