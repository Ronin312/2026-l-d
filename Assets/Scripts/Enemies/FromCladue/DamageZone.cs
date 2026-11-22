using System.Collections.Generic;
using UnityEngine;
using FrostGame.Health;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Znacznik na ziemi po zamachu Strażnika. Collider2D z włączonym Is Trigger.
    /// Rani każdy obiekt IDamageable z tagiem "Player" jeden raz i znika po czasie życia.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DamageZone : MonoBehaviour
    {
        [SerializeField] int damage = 1;
        [SerializeField] float lifetime = 1.5f;

        readonly HashSet<IDamageable> alreadyHit = new HashSet<IDamageable>();

        void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
            Destroy(gameObject, lifetime);
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null || !alreadyHit.Add(damageable)) return;
            damageable.TakeDamage(damage);
        }
    }
}