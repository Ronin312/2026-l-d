using UnityEngine;

namespace FrostGame.PlayerStuf
{
    public class CameraFollowTarget : MonoBehaviour
    {
        public Transform target;
        [SerializeField] private float smoothTime = 0.12f;

        private Vector3 velocity;
        private Vector3 cameraTargetPos;

        private void Start()
        {
            cameraTargetPos.z = transform.position.z;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            cameraTargetPos.x = target.position.x;
            cameraTargetPos.y = target.position.y;  
            transform.position = Vector3.SmoothDamp(transform.position, cameraTargetPos, ref velocity, smoothTime);
        }
    }
}
