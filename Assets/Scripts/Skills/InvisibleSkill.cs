using FrostGame.Enemies;
using FrostGame.Weapon;
using Unity.VisualScripting;
using UnityEngine;
namespace FrostGame.Skills
{
    public class InvisibleSkill : SkillWithSecondTimer
    {
        [SerializeField] protected SpriteRenderer spriteRend;
        [SerializeField] protected Transform playerTarget;

        protected Color invisibleColor;
        protected Color visibleColor;

        private void Start()
        {
            visibleColor = spriteRend.color;
            invisibleColor = visibleColor;
            invisibleColor.a = 0.5f;
            
        }
        protected override void Activate()
        {
            
            EnemyBase.target = null;
            workTimer = 0;
            spriteRend.color = invisibleColor;
            skillRun = true;
        }

        protected override void Disactivate()
        {
            skillRun = false;
            EnemyBase.target = playerTarget;
            spriteRend.color = visibleColor;
        }
    }
}
