using FrostGame.Scriptable;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FrostGame.PlayerStuf
{
    /// <summary>
    /// Pusty skrypt kontrolera gracza.
    /// </summary>
    //[RequireComponent(typeof(Rigidbody2D))]
    
    public class PlayerControl : MonoBehaviour
    {
        //[SerializeField]
        //UnityEvent<bool> myEvent;
        public Action<bool> OnAttackChanged;
        public Action<bool> OnAttack2Changed;
        public event Action OnInteractAction;
        public event Action OnSkillAction;
        public event Action OnExitAction;

        public event Action<Vector2> OnMove;

        //[SerializeField]
        //public Action<bool> Akcja;
        //protected Rigidbody2D rb;
        //[SerializeField] private float moveSpeed = 5f;
        protected bool attackPressed = false;
        protected bool lastAttackPressed = false;
        protected bool attack2Pressed = false;
        protected bool lastAttack2Pressed = false;
        //protected Vector2 inputVector = Vector2.zero;
        protected PlayerInput playerInput;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //rb = GetComponent<Rigidbody2D>();
            playerInput = GetComponent<PlayerInput>();
            playerInput.actions = FrostGameDataLoader.LoadActions();
        }

        #region Input System
        //są to funkcje powiązane z Input System, które są wywoływane automatycznie przez komponent Player Input (Send Messages)
        public void OnControl(InputValue value)
        {
            //inputVector = value.Get<Vector2>();
            OnMove?.Invoke(value.Get<Vector2>());
        }

        public void OnAttack1(InputValue value)
        {
            
        }

        public void OnAttack2(InputValue value)
        {

        }

        public void OnInteract(InputValue value)
        {
            float interact = value.Get<float>();
            OnInteractAction?.Invoke();
            //Debug.Log("Interact Value: " + interact);
        }

        public void OnSkill(InputValue value)
        {
            float skill = value.Get<float>();
            OnSkillAction?.Invoke();
            //Debug.Log("Skill Value: " + skill);
        }

        public void OnExit()
        {
            OnExitAction?.Invoke();
        }

        public void Atack1and2Actions()
        {
            //Vector2 vec = playerInput.actions["Control"].ReadValue<Vector2>();
            //Debug.Log("Move Vector: " + vec);
            //Debug.Log("Input Vector: " + inputVector);
            //float attackValue = playerInput.actions["Attack"].ReadValue<float>();
            //Debug.Log("Attack Value: " + attackValue);
            attackPressed = playerInput.actions["attack1"].IsPressed(); 
            if(lastAttackPressed != attackPressed)
            {
                OnAttackChanged?.Invoke(attackPressed);
                lastAttackPressed = attackPressed;
                //myEvent?.Invoke(attackPressed);
            }

            attack2Pressed = playerInput.actions["attack2"].IsPressed();
            if (lastAttack2Pressed != attack2Pressed)
            {
                OnAttack2Changed?.Invoke(attack2Pressed);
                lastAttack2Pressed = attack2Pressed;
                //myEvent?.Invoke(attackPressed);
            }
            //Debug.Log("Attack Pressed: " + attackPressed);
        }

        #endregion

        private void LateUpdate()
        {
            Atack1and2Actions();

        }

        //private void FixedUpdate()
        //{
        //    rb.linearVelocity = inputVector * moveSpeed;
        //}
    }
}
