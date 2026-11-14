using UnityEngine;
namespace FrostGame.Skills
{

    public class RapidFireSkill : SkillWithSecondTimer
    {
        [SerializeField] protected Weapon.Weapon distanceWeapon;
        [SerializeField] protected Weapon.Weapon meleWeapon;

        [SerializeField] protected float coldownScale = 0.5f;

        protected float distanceWeaponCooldown, meleWeaponCooldown;

        public override void SetSkill()
        {
            base.SetSkill();
            distanceWeaponCooldown = distanceWeapon.weaponColdown;
            meleWeaponCooldown = meleWeapon.weaponColdown;
        }

        protected override void Activate()
        {
            workTimer = 0;
            distanceWeapon.weaponColdown *= coldownScale;
            meleWeapon.weaponColdown *= coldownScale;
            skillRun = true;
        }

        protected override void Disactivate()
        {
            skillRun = false;
            distanceWeapon.weaponColdown = distanceWeaponCooldown;
            meleWeapon.weaponColdown = meleWeaponCooldown;
        }

    }
}
