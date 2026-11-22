using FrostGame.PlayerStuf;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace FrostGame.Skills
{

    public class TeleportSkill : Skill
    {
        //[SerializeField] private Rigidbody2D rb;
        [SerializeField] private PlayerControl pc;
        

        Vector2 helpPos = Vector2.zero;

        public override void SetSkill()
        {
            base.SetSkill();
            pc.OnPoint += HandlePoint;
        }

        private void HandlePoint(Vector2 vector)
        {
            helpPos = vector;
        }

        public override void RemoveSkill()
        {
            base.RemoveSkill();
            pc.OnPoint -= HandlePoint;
        }
        protected override void Activate()
        {
            helpPos = Camera.main.ScreenToWorldPoint(helpPos);
            //pc.transform.position = helpPos;
            //NavMesh.SamplePosition(helpPos, out NavMeshHit hit, 1f, NavMesh.AllAreas);
            if (NavMesh.SamplePosition(helpPos, out NavMeshHit hit, 1, NavMesh.AllAreas))
            {
                pc.transform.position = hit.position; // Zwraca najbliższy punkt na NavMesh
            }
        }

        protected override void Disactivate()
        {

        }

        protected override void FixedHook()
        {

        }
    }
}
