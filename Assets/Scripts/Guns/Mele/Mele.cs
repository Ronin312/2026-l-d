
using UnityEngine;
using UnityEngine.InputSystem;
using FrostGame.PlayerStuf;
using System;

namespace FrostGame.Gun
{

    public class Mele : MonoBehaviour
    {
        [Header("Miecz")]
        [SerializeField] private float swordDamage = 25f;
        [SerializeField] private float swordRange = 1.2f;
        [SerializeField] private float swordRadius = 0.8f;
        [SerializeField] private float swordCooldown = 0.4f;
        [SerializeField] private LayerMask enemyMask;

        bool swordPressed = false;


       [SerializeField] private PlayerControl controller;
        private Camera cam;
        private float swordTimer;

        private void Awake()
        {
            cam = Camera.main;
            controller.OnAttack2Changed += OnSecondAttack;
        }

        private void OnSecondAttack(bool obj)
        {
            swordPressed = obj;
        }

        private void Update()
        {
            if (Time.timeScale == 0f) return; // pauza

            swordTimer -= Time.deltaTime;

            if (swordPressed) SwordAttack();
 
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

        private void SwordAttack()
        {
            if (swordTimer > 0f) return;
            swordTimer = swordCooldown;

            Vector2 center = (Vector2)transform.position + AimDirection() * swordRange;
            SlashEffect.Spawn(center, swordRadius, new Color(1f, 1f, 1f, 0.55f));
            var hits = Physics2D.OverlapCircleAll(center, swordRadius, enemyMask);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out FrostGame.Health.Health health))
                    health.TakeDamage(swordDamage);
            }
        }

        //private void Shoot()
        //{
        //    if (shootTimer > 0f || projectilePrefab == null) return;
        //    shootTimer = shootCooldown;

        //    Vector2 dir = AimDirection();
        //    Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        //    var p = Instantiate(projectilePrefab, origin, Quaternion.identity);
        //    p.Launch(dir, projectileDamage);
        //}

        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position + (Vector3)(Vector2.down * swordRange), swordRadius);
        //}
    }
}
