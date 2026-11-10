using UnityEngine;
namespace FrostGame.PlayerStuf
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerControl))]
    public class PlayerMove : MonoBehaviour
    {
        protected Rigidbody2D rb;
        protected PlayerControl playerControl;
        protected Vector2 inputVector = Vector2.zero;
   
        [SerializeField] protected float moveSpeed = 5f;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            playerControl = GetComponent<PlayerControl>();
            playerControl.OnMove += HandleMove;
        }
        protected virtual void HandleMove(Vector2 inVector)
        {
            inputVector = inVector * moveSpeed;
            //rb.linearVelocity = inputVector;
        }


        protected void FixedUpdate()
        {
            rb.linearVelocity = inputVector;
           
        }


    }
}

