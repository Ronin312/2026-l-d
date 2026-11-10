using FrostGame.PlayerStuf;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace FrostGame.Skills
{
    public class SkillActivator : MonoBehaviour
    {
        [SerializeField] private Color activeColor = Color.green;
        [SerializeField] private Color notActiveColor = Color.red;

        [SerializeField] private Image image;
        [SerializeField] private Skill[] skill;
        private Skill lastSkill = null;
        private int lastSkilIndex = 0;
        [SerializeField] private PlayerControl playerControl;

        [SerializeField] private Image skillImage;

        [SerializeField] float skilChangeTimePosibility = 0.5f;
        float sCTPTimer = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            playerControl.OnSkillAction += HandleSkill;
            playerControl.OnScroolAction += HandleScrool;

            ChangeSkil(0);
        }

        private void FixedUpdate()
        {
            if (sCTPTimer < skilChangeTimePosibility)
            {
                sCTPTimer += Time.fixedDeltaTime;
            }

        }

        void ChangeSkil(int to)
        {
            if (lastSkill != null)
            {
                lastSkill.OnTimerUpdate -= HandleTimerUpdate;
                lastSkill.RemoveSkill();

            }
            lastSkill = skill[to];
            lastSkilIndex = to;
            lastSkill.OnTimerUpdate += HandleTimerUpdate;
            lastSkill.SetSkill();
            skillImage.sprite = lastSkill.GetSkillSprite();
        }

        private void HandleScrool(bool obj)
        {
            if(sCTPTimer < skilChangeTimePosibility)
                return;

            sCTPTimer = 0;
            if (obj)
            {
                lastSkilIndex++;
                if (lastSkilIndex >= skill.Length)
                    lastSkilIndex = 0;
            }
            else
            {
                lastSkilIndex--;
                if (lastSkilIndex < 0)
                    lastSkilIndex = skill.Length - 1;
            }

            ChangeSkil(lastSkilIndex);
        }

        private void HandleTimerUpdate(float arg1, float arg2)
        {
           image.color = arg1 >= arg2 ? activeColor : notActiveColor;
           image.fillAmount = arg1 / arg2;
        }

        private void HandleSkill()
        {
            if(lastSkill != null)
            lastSkill.TryActivate();
        }


    }
}
