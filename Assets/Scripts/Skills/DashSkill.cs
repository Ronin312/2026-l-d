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
            workTimer = howLongSkillWorks;
        }

        protected override void Disactivate()
        {
            skillRun = false;
            playerMoveWithDash.enabled = true;
        }
    }
}
