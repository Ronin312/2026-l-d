using UnityEngine;

namespace FrostGame.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Echo Phantom Enemy Data")]
    public class EchoPhantomEnemyData : ScriptableObject
    {
        [Header("Echo phantom | Echo")]
        public float echoDelay = 2f;
        public float echoSampleInterval = 0.1f;
        public float echoDisperseTime = 1.2f;
        public float echoSpeed = 6f;
    }
}
