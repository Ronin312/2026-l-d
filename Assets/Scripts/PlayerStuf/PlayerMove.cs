using UnityEngine;
namespace FrostGame.PlayerStuf
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerControl))]
    public class PlayerMove : MonoBehaviour
    {
        private Rigidbody2D rb;
        private PlayerControl playerControl;
        [SerializeField] private float moveSpeed = 5f;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            playerControl = GetComponent<PlayerControl>();
            playerControl.OnMove += HandleMove;
        }
        private void HandleMove(Vector2 inputVector)
        {
            rb.linearVelocity = inputVector * moveSpeed;
        }
    }
}

