using UnityEngine;

namespace FrostGame.Health
{
    /// <summary>
    /// Minimalny przykład gracza do testów wrogów. Jeśli masz już własny system zdrowia,
    /// wystarczy, że zaimplementuje IDamageable, a obiekt gracza ma tag "Player".
    /// </summary>
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] int maxHealth = 10;
        [SerializeField] float invulnerabilityTime = 0.5f;

        int currentHealth;
        float invulnerableUntil;

        void Awake() => currentHealth = maxHealth;

        public void TakeDamage(int amount)
        {
            if (Time.time < invulnerableUntil) return;

            currentHealth -= amount;
            invulnerableUntil = Time.time + invulnerabilityTime;
            Debug.Log($"Gracz: {currentHealth}/{maxHealth} HP");

            if (currentHealth <= 0)
            {
                Debug.Log("Gracz zginął");
                gameObject.SetActive(false);
            }
        }
    }
}