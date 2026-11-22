using System;
using UnityEngine;

namespace FrostGame.Health
{
    /// <summary>
    /// Uniwersalny komponent zdrowia dla gracza, potworów i bossów.
    /// </summary>
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float invulnerabilityTime = 0f;

        public int Current { get; private set; }
        public int Max => maxHealth;
        public bool IsDead { get; private set; }

        public bool IsImmortal = false;

        public event Action<float, float> OnChanged; // (aktualne, maksymalne)
        public event Action<Health> OnDied;

        private float lastHitTime = -999f;

        private void Awake()
        {
            Current = maxHealth;
        }

        public void SetInvulnerability(float seconds) => invulnerabilityTime = seconds;

        public void SetMax(int value, bool refill = true)
        {
            maxHealth = value;
            if (refill) Current = maxHealth;
            OnChanged?.Invoke(Current, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (IsImmortal) return;
            if (IsDead) return;
            if (Time.time - lastHitTime < invulnerabilityTime) return;

            lastHitTime = Time.time;
            Current = Mathf.Max(0, Current - amount);
            OnChanged?.Invoke(Current, maxHealth);

            if (Current <= 0f)
            {
                IsDead = true;
                OnDied?.Invoke(this);
            }
        }

        public void Heal(int amount)
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
