using UnityEngine;

namespace FrostGame.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Horned Bull Enemy Data")]
    public class HornedBullEnemyData : ScriptableObject
    {
        [Header("Horned Bull | Rogaty Byk")]
        public float bullChargeRange = 7f;
        public float bullWindupTime = 1f;
        public float bullChargeSpeed = 10f;
        public int bullChargeDamage = 3;
        public float bullMaxChargeTime = 1.2f;
        public float bullChargeCooldown = 2f;
        public float bullStunDuration = 2.5f;
        public float bullStunDamageMultiplier = 2f;

    }
}
