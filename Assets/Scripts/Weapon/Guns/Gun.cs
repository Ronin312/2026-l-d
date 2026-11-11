
using UnityEngine;
using UnityEngine.InputSystem;

namespace FrostGame.Weapon
{
    public class Gun : Weapon
    {
        [Header("Strzał")]
        [SerializeField] private Bullet.Bullet projectilePrefab;
        private Transform firePoint;

        private Camera cam;

        private void Awake()
        {
            firePoint ??= transform;
            cam = Camera.main;
            controller.OnAttackChanged += OnAttack;
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
            if (wcTimer > 0f || projectilePrefab == null) return;
            wcTimer = weaponColdown;

            Vector2 dir = AimDirection();
            Vector3 origin = firePoint != null ? firePoint.position : transform.position;
            var p = Instantiate(projectilePrefab, origin, Quaternion.identity);
            p.Launch(dir, damage);
        }

        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position + (Vector3)(Vector2.down * swordRange), swordRadius);
        //}
    }
}
