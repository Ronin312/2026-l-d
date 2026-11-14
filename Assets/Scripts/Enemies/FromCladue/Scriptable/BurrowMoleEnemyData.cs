using UnityEngine;

namespace FrostGame.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Burrow Mole Enemy Data")]
    public class BurrowMoleEnemyData : ScriptableObject
    {
        [Header("Burrow Mole | Kret Zasadzkowiec")]
        public float moleTelegraphTime = 1f;
        public float moleTriggerDistance = 0.6f;
        public float moleBurstRadius = 1.6f;
        public int moleBurstDamage = 2;
        public float moleSurfacedTime = 3f;
        public float moleUndergroundMaxTime = 6f;

    }
}
