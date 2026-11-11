using UnityEngine;
using UnityEngine.InputSystem;

namespace FrostGame.Weapon
{
    public class Mele : Weapon
    {
        [Header("Miecz")]
        [SerializeField] private float swordRange = 1.2f;
        [SerializeField] private float swordRadius = 0.8f;
        [SerializeField] private LayerMask enemyMask;
        private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
            controller.OnAttack2Changed += OnAttack;
        }


        private Vector2 AimDirection()
        {
            if (cam == null) cam = Camera.main;
            var mouse = Mouse.current;
            if (cam != null && mouse != null)
            {
                Vector2 world = cam.ScreenToWorldPoint(mouse.position.ReadValue());
                Vector2 dir = world - (Vector2)transform.position;
                if (dir.sqrMagnitude > 0.01f) return dir.normalized;
            }
            return controller.transform.right;
        }

        protected override void Attack()
        {
            if (wcTimer > 0f) return;
            wcTimer = weaponColdown;

            Vector2 center = (Vector2)transform.position + AimDirection() * swordRange;
            SlashEffect.Spawn(center, swordRadius, new Color(1f, 1f, 1f, 0.55f));
            var hits = Physics2D.OverlapCircleAll(center, swordRadius, enemyMask);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out FrostGame.Health.Health health))
                    health.TakeDamage(damage);
            }
        }

        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position + (Vector3)(Vector2.down * swordRange), swordRadius);
        //}
    }
}
