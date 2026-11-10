using FrostGame.Weapon;
using UnityEngine;
namespace FrostGame.Skills
{

    public class ShieldSkill : Skill
    {
        [SerializeField] protected Health.Health health;

        [Header("Shield")]
        [SerializeField] private float shieldTime = 5f;
        private float shieldTimerek = 0;
        bool skillRun = false;
        GameObject shieldGob;
        protected override void Activate()
        {
            
            health.IsImmortal = true;
            shieldTimerek = 0;
            shieldGob = SlashEffect.Spawn(shieldTime *2, health.transform, .7f, new Color(0,1,0,0.2f));
            skillRun = true;
        }

        protected override void Disactivate()
        {
            skillRun = false;
            health.IsImmortal = false;
            Destroy(shieldGob);
        }

        protected override void FixedHook()
        {
            if (skillRun)
            {
                shieldTimerek += Time.fixedDeltaTime;
                if(shieldTimerek >=shieldTime)
                {
                    Disactivate();
                }
            }
        }
    }
}
