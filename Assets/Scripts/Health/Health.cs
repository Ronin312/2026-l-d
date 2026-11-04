using System;
using UnityEngine;

namespace FrostGame.Health
{
    /// <summary>
    /// Uniwersalny komponent zdrowia dla gracza, potworów i bossów.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float invulnerabilityTime = 0f;

        public float Current { get; private set; }
        public float Max => maxHealth;
        public bool IsDead { get; private set; }

        public event Action<float, float> OnChanged; // (aktualne, maksymalne)
        public event Action<Health> OnDied;

        private float lastHitTime = -999f;

        private void Awake()
        {
            Current = maxHealth;
        }

        public void SetInvulnerability(float seconds) => invulnerabilityTime = seconds;

        public void SetMax(float value, bool refill = true)
        {
            maxHealth = value;
            if (refill) Current = maxHealth;
            OnChanged?.Invoke(Current, maxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead) return;
            if (Time.time - lastHitTime < invulnerabilityTime) return;

            lastHitTime = Time.time;
            Current = Mathf.Max(0f, Current - amount);
            OnChanged?.Invoke(Current, maxHealth);

            if (Current <= 0f)
            {
                IsDead = true;
                OnDied?.Invoke(this);
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            Current = Mathf.Min(maxHealth, Current + amount);
            OnChanged?.Invoke(Current, maxHealth);
        }

        public void Revive()
        {
            IsDead = false;
            Current = maxHealth;
            OnChanged?.Invoke(Current, maxHealth);
        }
    }
}
