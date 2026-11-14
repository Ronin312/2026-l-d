using UnityEngine;

namespace FrostGame.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Swarm Spider Enemy Data")]
    public class SwarmSpiderEnemyData : ScriptableObject
    {
        [Header("Swarm Spider | Rojowy Pająk")]
        public float chargeSpeedMultiplier = 2.2f;
        public float cocoonHatchTime = 3f;

    }
}
