using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Zarządza rojem pająków: spawnuje grupę, rozdziela kąty okrążania (żeby się nie nakładały),
    /// a potem wydaje rozkaz szarży dla wszystkich naraz. Powtarza cykl w pętli.
    /// </summary>
    public class SwarmManager : MonoBehaviour
    {
        [Header("Roj")]
        [SerializeField] SwarmSpider spiderPrefab;
        [SerializeField] int initialCount = 5;
        [SerializeField] int maxAlive = 8;
        [SerializeField] float spawnRadius = 1f;

        [Header("Cykl ataku")]
        [SerializeField] float orbitRadius = 4f;
        [SerializeField] float orbitAngularSpeed = 60f;
        [SerializeField] float orbitDuration = 3f;
        [SerializeField] float chargeDuration = 1.2f;

        readonly List<SwarmSpider> spiders = new List<SwarmSpider>();
        SwarmSpider.Mode currentPhase = SwarmSpider.Mode.Orbit;

        void Start()
        {
            for (int i = 0; i < initialCount; i++)
                SpawnSpider((Vector2)transform.position + Random.insideUnitCircle * spawnRadius);

            StartCoroutine(AttackCycle());
        }

        /// <summary>Wołane przy starcie i przez kokon przy wykluciu.</summary>
        public void SpawnSpider(Vector2 position)
        {
            if (spiderPrefab == null || spiders.Count >= maxAlive) return;

            var spider = Instantiate(spiderPrefab, position, Quaternion.identity);
            spider.Setup(this);
            spider.Died += OnSpiderDied;
            spiders.Add(spider);

            Reassign();
        }

        void OnSpiderDied(EnemyBase enemy)
        {
            spiders.Remove(enemy as SwarmSpider);
            Reassign();
        }

        IEnumerator AttackCycle()
        {
            while (true)
            {
                currentPhase = SwarmSpider.Mode.Orbit;
                Reassign();
                yield return new WaitForSeconds(orbitDuration);

                currentPhase = SwarmSpider.Mode.Charge;
                Reassign();
                yield return new WaitForSeconds(chargeDuration);
            }
        }

        /// <summary>Rozkłada pająki równomiernie po okręgu i aplikuje bieżącą fazę.</summary>
        void Reassign()
        {
            spiders.RemoveAll(s => s == null);

            for (int i = 0; i < spiders.Count; i++)
            {
                float angle = 360f * i / spiders.Count;

                if (currentPhase == SwarmSpider.Mode.Orbit)
                    spiders[i].SetOrbit(angle, orbitRadius, orbitAngularSpeed);
                else
                    spiders[i].SetCharge();
            }
        }
    }
}