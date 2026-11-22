using UnityEngine;

namespace FrostGame.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Shadow Archer Enemy Data")]
    public class ShadowArcherEnemyData : ScriptableObject
    {
        [Header("Shadow Archer | Cienisty Łucznik")]
        public float preferredDistance = 6f;
        public float distanceTolerance = 1f;
        public float panicDistance = 3f;
        public float shotCooldown = 2.5f;
        public float aimTime = 0.8f;
        public float projectileSpeed = 8f;
        public int projectileDamage = 1;
        public float teleportCooldown = 5f;
    }
}
