using FrostGame.Enemies;
using Unity.VisualScripting;
using UnityEngine;

namespace FrostGame.Enemies
{
    public abstract class EnemyBaseWithAnim : EnemyBase
    {
        protected Animator anim;
        protected Collider2D col;

        protected override void Awake()
        {
            base.Awake();
            col = GetComponent<Collider2D>();
            offset = new Vector3(col.offset.x, col.offset.y, 0);
            anim = GetComponent<Animator>();
            DiedAction += DieAnimation;
        }

        protected virtual void DieAnimation(EnemyBase @base)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            col.enabled = false;
            anim.Play("Die");

        }

    }
}
