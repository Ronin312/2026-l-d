using UnityEngine;
namespace FrostGame.PlayerStuf
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerControl))]
    public class PlayerMoveWithDash : PlayerMove
    {
        private Vector2 lastMovableVector = Vector2.right;

        protected override void HandleMove(Vector2 inVector)
        {
            base.HandleMove(inVector);
            if (inputVector != Vector2.zero)
            {
                lastMovableVector = inputVector;
                lastMovableVector.Normalize();
            }
        }

        public Vector2 GetLastMovableVector()
        {
            return lastMovableVector;
        }

        private void OnDisable()
        {
            inputVector = Vector2.zero;
        }

    }
}

