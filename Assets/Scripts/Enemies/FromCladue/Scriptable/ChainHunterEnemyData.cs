using UnityEngine;

namespace FrostGame.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Chain Hunter Enemy Data")]
    public class ChainHunterEnemyData : ScriptableObject
    {
        [Header("Chain hunter | Łowca z Łańcuchem")]
        public float hookKeepDistance = 6f;
        public float hookRange = 10f;
        public float hookTelegraphTime = 0.6f;
        public float hookSpeed = 18f;
        public float hookCooldown = 4f;
        public int hookDamage = 1;
        public float hookPullSpeed = 14f;
        public float hookPullDuration = 0.5f;
        public float hookStopDistance = 1.2f;
        public float hookBlockedStunTime = 2f;
        public float hookStunDamageMultiplier = 2f;
    }
}
