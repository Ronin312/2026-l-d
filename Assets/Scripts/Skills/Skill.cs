using System;
using UnityEngine;

namespace FrostGame.Skills
{
   
    public abstract class Skill : MonoBehaviour
    {
        public event Action<float, float> OnTimerUpdate;
        protected bool isActive = false;
        [SerializeField] protected float skillCooldown = 1f;
        [SerializeField] protected Sprite skillSprite;
        protected float skilCooldownTimer = 0;

        public Sprite GetSkillSprite() => skillSprite;

        public virtual void SetSkill()
        {
            isActive = true;
            skilCooldownTimer = 0;
        }

        public virtual void RemoveSkill()
        {
            Disactivate();
            isActive = false;
        }


        public void FixedUpdate()
        {
            if (isActive)
            {
                //tylko liczę czas
                skilCooldownTimer += Time.fixedDeltaTime;
                OnTimerUpdate!.Invoke(skilCooldownTimer, skillCooldown);

                FixedHook();
            }
        }

        protected abstract void FixedHook();

        public void TryActivate()
        {
            if (skilCooldownTimer >= skillCooldown)
            {
                Activate();
                skilCooldownTimer = 0;
            }
        }
        abstract protected void Activate();
        abstract protected void Disactivate();
    }
}
