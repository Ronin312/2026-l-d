using FrostGame.PlayerStuf;
using UnityEngine;
namespace FrostGame.Skills
{

    public class DashSkill : SkillWithSecondTimer
    {
        [SerializeField] protected PlayerMoveWithDash playerMoveWithDash;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float dashSpeed = 16f;
 

        protected override void Activate()
        {
            playerMoveWithDash.enabled = false;
            skillRun = true;
            workTimer = 0;
        }

        protected override void Disactivate()
        {
            skillRun = false;
            playerMoveWithDash.enabled = true;
        }

        protected override void FixedHook()
        {
            if (skillRun)
            {
                workTimer += Time.fixedDeltaTime;
                rb.linearVelocity = playerMoveWithDash.GetLastMovableVector() * dashSpeed;
                if (workTimer >= howLongSkillWorks)
                {
                    Disactivate();
                }
            }
        }
    }
}
