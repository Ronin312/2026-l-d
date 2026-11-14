using UnityEngine;

namespace FrostGame.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Stone Guardian Enemy Data")]
    public class StoneGuardianEnemyData : ScriptableObject
    {
        [Header("Stone Guardian | Kamienny Strażnik")]
        public int armorHealth = 15;
        public float attackRange = 1.8f;
        public float windupTime = 0.7f;
        public float slamCooldown = 2f;
        public int slamDamage = 2;
        public float slamRadius = 1.4f;
        public float stunDuration = 2f;
       
    }
}
