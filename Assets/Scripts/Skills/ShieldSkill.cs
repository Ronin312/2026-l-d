using FrostGame.Weapon;
using UnityEngine;
namespace FrostGame.Skills
{
    public class ShieldSkill : SkillWithSecondTimer
    {
        [SerializeField] protected Health.Health health;
        GameObject shieldGob;
        protected override void Activate()
        {
            
            health.IsImmortal = true;
            workTimer = 0;
            shieldGob = SlashEffect.Spawn(howLongSkillWorks * 2, health.transform, .7f, new Color(0,1,0,0.2f));
            skillRun = true;
        }

        protected override void Disactivate()
        {
            skillRun = false;
            health.IsImmortal = false;
            Destroy(shieldGob);
        }
    }
}
