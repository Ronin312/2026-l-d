using FrostGame.PlayerStuf;
using UnityEngine;
namespace FrostGame.Weapon
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected PlayerControl controller;
        [SerializeField] public float weaponColdown = 0.5f;
        [SerializeField] protected int damage = 5;
        [SerializeField] protected bool attacFromParnet = false;
        protected float wcTimer;


        protected bool attackPressed = false;

        private void Update()
        {
            if (Time.timeScale == 0f) return; // pauza

            wcTimer -= Time.deltaTime;

            if (attackPressed) Attack();
        }

        protected abstract void Attack();

        protected void OnAttack(bool obj)
        {
            attackPressed = obj;
        }
    }
}
