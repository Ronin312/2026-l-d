using System;
using UnityEngine;

namespace FrostGame.Skills
{
   
    public abstract class SkillWithSecondTimer : Skill
    {
        [SerializeField] protected float howLongSkillWorks = 1f;
        protected float workTimer = 0f;
        protected bool skillRun = false;

        protected override void FixedHook()
        {
            if (skillRun)
            {
                workTimer += Time.fixedDeltaTime;
                if (workTimer >= howLongSkillWorks)
                {
                    Disactivate();
                }
            }
        }
    }
}
