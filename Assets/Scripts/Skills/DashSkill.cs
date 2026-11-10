using FrostGame.PlayerStuf;
using UnityEngine;
namespace FrostGame.Skills
{

    public class DashSkill : Skill
    {
        [SerializeField] protected PlayerMoveWithDash playerMoveWithDash;
        [SerializeField] private Rigidbody2D rb;
        [Header("Dash")]
        [SerializeField] private float dashSpeed = 16f;
        [SerializeField] private float dashDuration = 0.15f;

        private float dashTimer;
        private bool dashOn = false;

        protected override void Activate()
        {
            playerMoveWithDash.enabled = false;
            dashOn = true;
            dashTimer = dashDuration;
        }

        protected override void Disactivate()
        {
            dashOn = false;
            playerMoveWithDash.enabled = true;
        }

        protected override void FixedHook()
        {
            if (dashOn)
            {
                rb.linearVelocity = playerMoveWithDash.GetLastMovableVector() * dashSpeed;
                dashTimer -= Time.fixedDeltaTime;
                if(dashTimer <= 0f)
                {
                    Disactivate();
                }
            }
        }
    }
}
