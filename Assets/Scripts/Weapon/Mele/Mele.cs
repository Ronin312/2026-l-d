using FrostGame.Health;
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

        [Header("Efekt cięcia")]
        [SerializeField] private RuntimeAnimatorController slashAnimator;
        [SerializeField] private float slashDuration = 0.14f;
        [SerializeField] private float slashAngleOffset = 0f;

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

            Vector2 dir = AimDirection();
            Vector2 center;
            if (attacFromParnet)
                center = (Vector2)transform.parent.position + dir * swordRange;
            else
                center = (Vector2)transform.position + dir * swordRange;

            bool mirrorRight = dir.x > 0f;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + slashAngleOffset;
            SlashEffect.Spawn(center, swordRadius, new Color(1f, 1f, 1f, 0.55f), slashAnimator, angle, slashDuration, mirrorRight);

            var hits = Physics2D.OverlapCircleAll(center, swordRadius, enemyMask);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out IDamageable health))
                    health.TakeDamage(damage);
            }
        }
    }
}
