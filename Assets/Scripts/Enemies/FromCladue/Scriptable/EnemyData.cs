using UnityEngine;

namespace FrostGame.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Shared | Wspólne")]
        public int maxHealth = 10;
        public float moveSpeed = 2f;
        public int contactDamage = 1;
        public float contactCooldown = 0.8f;
        public float detectionRange = 8f;
    }
}
