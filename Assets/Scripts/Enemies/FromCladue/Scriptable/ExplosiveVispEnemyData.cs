using UnityEngine;

namespace FrostGame.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Explosive Visp Enemy Data")]
    public class ExplosiveVispEnemyData : ScriptableObject
    {
        [Header("Explosive Visp | Ognik Wybuchowy")]
        public float wispTriggerRange = 1.8f;
        public float wispFuseTime = 0.8f;
        public float wispBlastRadius = 2.2f;
        public int wispBlastDamage = 3;
        public float wispWobble = 0.6f;
        public float wispWobbleFrequency = 3f;
    }
}
