using UnityEngine;

namespace FrostGame.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Rune Shaman Enemy Data")]
    public class RuneShamanEnemyData : ScriptableObject
    {
        [Header("Rune Shaman | Szaman Run")]
        public float shamanKeepDistance = 7f;
        public float shieldInterval = 4f;
        public int shieldHits = 3;
        public float shieldRange = 8f;
        public float summonCooldown = 6f;
        public int summonCount = 2;
        public float spiritLifetime = 6f;
    }
}
