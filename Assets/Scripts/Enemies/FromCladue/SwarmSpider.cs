using UnityEngine;
namespace FrostGame.Enemies
{
    /// <summary>
    /// Rojowy Pająk: słaby, działa w grupie. Rozkazy (okrążanie / szarża) dostaje od SwarmManager.
    /// Po śmierci zostawia kokon, z którego wykluwa się nowy pająk.
    /// </summary>
    public class SwarmSpider : EnemyBaseWithAnim
    {
        public enum Mode { Idle, Orbit, Charge }

        [SerializeField] SpiderCocoon cocoonPrefab;
        [SerializeField] SwarmSpiderEnemyData swarSpiderEnemyData;
        SwarmManager manager;
        Mode mode = Mode.Idle;
        float orbitAngle;
        float orbitRadius = 4f;
        float orbitAngularSpeed = 60f;

        public void Setup(SwarmManager owner) => manager = owner;

        public void SetOrbit(float startAngle, float radius, float angularSpeed)
        {
            mode = Mode.Orbit;
            orbitAngle = startAngle;
            orbitRadius = radius;
            orbitAngularSpeed = angularSpeed;
        }

        public void SetCharge() => mode = Mode.Charge;

        public void SetIdle() => mode = Mode.Idle;

        protected override void Tick()
        {
            if (target == null) return;

            switch (mode)
            {
                case Mode.Orbit:
                    orbitAngle += orbitAngularSpeed * Time.deltaTime;
                    Vector2 offset = Quaternion.Euler(0f, 0f, orbitAngle) * Vector2.right * orbitRadius;
                    Vector2 desired = (Vector2)target.position + offset;
                    Vector2 toDesired = desired - rb.position;
                    if (toDesired.magnitude > 0.1f) Move(toDesired, data.moveSpeed);
                    break;

                case Mode.Charge:
                    Move(DirectionToTarget, data.moveSpeed * swarSpiderEnemyData.chargeSpeedMultiplier);
                    break;
            }
        }

        protected override void Die()
        {
            if (isDead) return;

            if (cocoonPrefab != null && manager != null)
            {
                var cocoon = Instantiate(cocoonPrefab, transform.position, Quaternion.identity);
                cocoon.Init(manager, swarSpiderEnemyData.cocoonHatchTime);
            }

            base.Die();
        }
    }
}