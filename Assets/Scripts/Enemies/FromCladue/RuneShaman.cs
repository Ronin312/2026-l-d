using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FrostGame.Enemies
{
    /// <summary>
    /// Szaman Run: wsparcie. Trzyma dystans od gracza, nakłada tarcze na sojuszników
    /// (najpierw na tego, który jest najbliżej gracza), a gdy nie ma kogo osłaniać, przywołuje duchy.
    /// Śmierć szamana zdejmuje wszystkie tarcze, które nałożył.
    /// </summary>
    public class RuneShaman : EnemyBase
    {
        [SerializeField] RuneShamanEnemyData runeShamanEnemyData;
        [Header("Tarcze")]
        [SerializeField] LayerMask enemyLayer;
        [SerializeField] LineRenderer beam;
        [SerializeField] float beamShowTime = 0.4f;

        [Header("Przywołanie")]
        [SerializeField] SpiritMinion spiritPrefab;
        [SerializeField] float summonRadius = 1.2f;
        [SerializeField] int maxSpirits = 4;

        readonly List<GameObject> shielded = new List<GameObject>();
        readonly List<GameObject> spirits = new List<GameObject>();
        float nextShieldTime;
        float nextSummonTime;

        protected override void Start()
        {
            base.Start();
            if (beam != null) beam.enabled = false;
            nextShieldTime = Time.time + 1f;
            nextSummonTime = Time.time + 2f;
        }

        protected override void Tick()
        {
            if (target == null) return;

            float dist = DistanceToTarget;
            if (dist > data.detectionRange) return;

            if (dist < runeShamanEnemyData.shamanKeepDistance)
                Move(-DirectionToTarget, data.moveSpeed);
            else if (dist > runeShamanEnemyData.shamanKeepDistance + 2f)
                Move(DirectionToTarget, data.moveSpeed * 0.6f);

            if (Time.time >= nextShieldTime)
            {
                var ally = FindShieldTarget();
                if (ally != null)
                {
                    ShieldBuff.Apply(ally.gameObject, runeShamanEnemyData.shieldHits);
                    shielded.Add(ally.gameObject);
                    nextShieldTime = Time.time + runeShamanEnemyData.shieldInterval;
                    if (beam != null) StartCoroutine(ShowBeam(ally.transform));
                }
                else if (Time.time >= nextSummonTime && spiritPrefab != null)
                {
                    Summon();
                }
            }
        }

        EnemyBase FindShieldTarget()
        {
            var hits = Physics2D.OverlapCircleAll(rb.position, runeShamanEnemyData.shieldRange, enemyLayer);
            EnemyBase best = null;
            float bestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                var enemy = hit.GetComponentInParent<EnemyBase>();
                if (enemy == null || enemy == this || enemy.IsDead) continue;
                if (enemy.GetComponent<ShieldBuff>() != null) continue;
                if (spirits.Contains(enemy.gameObject)) continue;

                float d = Vector2.Distance(enemy.transform.position, target.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = enemy;
                }
            }

            return best;
        }

        void Summon()
        {
            spirits.RemoveAll(s => s == null);
            if (spirits.Count + runeShamanEnemyData.summonCount > maxSpirits)
            {
                nextSummonTime = Time.time + 1f;
                return;
            }

            for (int i = 0; i < runeShamanEnemyData.summonCount; i++)
            {
                Vector2 pos = rb.position + Random.insideUnitCircle * summonRadius;
                var spirit = Instantiate(spiritPrefab, pos, Quaternion.identity);
                spirit.Init(runeShamanEnemyData.spiritLifetime);
                spirits.Add(spirit.gameObject);
            }

            nextSummonTime = Time.time + runeShamanEnemyData.summonCooldown;
        }

        IEnumerator ShowBeam(Transform to)
        {
            beam.enabled = true;
            float t = 0f;
            while (t < beamShowTime && to != null)
            {
                beam.SetPosition(0, transform.position);
                beam.SetPosition(1, to.position);
                t += Time.deltaTime;
                yield return null;
            }
            if (beam != null) beam.enabled = false;
        }

        protected override void Die()
        {
            if (isDead) return;

            foreach (var go in shielded)
            {
                if (go != null && go.TryGetComponent(out ShieldBuff shield))
                    shield.Remove();
            }

            base.Die();
        }

        void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, runeShamanEnemyData.shieldRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, runeShamanEnemyData.shamanKeepDistance);
        }
    }
}
