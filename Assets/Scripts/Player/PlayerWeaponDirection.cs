using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerWeaponDirection : MonoBehaviour
{
    [Header("Animator & Parameters")]
    [SerializeField] private Animator animator;
    [SerializeField] private string moveXParam = "MoveX";
    [SerializeField] private string moveYParam = "MoveY";
    [SerializeField] private string weaponStateParam = "WeaponState";

    [Header("Weapons")]
    [SerializeField] private GameObject firstGun;
    [SerializeField] private GameObject firstMele;

    [Header("Combat & Aiming")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float combatStanceDuration = 0.35f;
    [SerializeField] private bool stayInCombatWhileHolding = true;

    [Header("Attack Events (Optional)")]
    [SerializeField] private UnityEvent onRangedAttack;
    [SerializeField] private UnityEvent onMeleeAttack;

    private Vector2 facingDir = Vector2.down;
    private int currentWeaponState = 0; // 0 = idle, 1 = ranged (LPM), 2 = melee (PPM)
    private float attackTimer = 0f;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        SetWeaponState(0);
    }

    private void Update()
    {
        HandleAttackAndCombatTimer();
        HandleFacingDirection();
        PushAnimatorValues();
    }

    private void HandleAttackAndCombatTimer()
    {
        var mouse = Mouse.current;
        bool pressedRanged = mouse != null && mouse.leftButton.wasPressedThisFrame;
        bool pressedMelee = mouse != null && mouse.rightButton.wasPressedThisFrame;
        bool holdingAnyMouse = stayInCombatWhileHolding && mouse != null &&
                               (mouse.leftButton.isPressed || mouse.rightButton.isPressed);

        if (pressedRanged)
        {
            SetWeaponState(1);
            attackTimer = combatStanceDuration;
            onRangedAttack?.Invoke();
        }
        else if (pressedMelee)
        {
            SetWeaponState(2);
            attackTimer = combatStanceDuration;
            onMeleeAttack?.Invoke();
        }
        else if (currentWeaponState != 0)
        {
            if (holdingAnyMouse)
            {
                attackTimer = Mathf.Max(attackTimer, 0.05f);
            }
            else
            {
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    attackTimer = 0f;
                    SetWeaponState(0);
                }
            }
        }
    }

    private void HandleFacingDirection()
    {
        bool isFighting = currentWeaponState != 0;

        if (isFighting && targetCamera != null && Mouse.current != null)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            float depth = Mathf.Abs(targetCamera.transform.position.z - transform.position.z);
            Vector3 mouseWorld = targetCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, depth));
            Vector2 toMouse = mouseWorld - transform.position;

            if (toMouse.sqrMagnitude > 0.001f)
            {
                facingDir = toMouse.normalized;
            }
            return;
        }

        // Default idle / non-fighting direction follows movement input
        var kb = Keyboard.current;
        if (kb == null)
        {
            return;
        }

        float x = 0f;
        float y = 0f;

        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) y -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) y += 1f;

        Vector2 moveInput = new Vector2(x, y);
        if (moveInput.sqrMagnitude > 0.01f)
        {
            facingDir = moveInput.normalized;
        }
    }

    private void PushAnimatorValues()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat(moveXParam, facingDir.x);
        animator.SetFloat(moveYParam, facingDir.y);
        animator.SetInteger(weaponStateParam, currentWeaponState);
    }

    public void SetWeaponState(int state)
    {
        currentWeaponState = state;

        if (firstGun != null)
        {
            firstGun.SetActive(state == 1);
        }

        if (firstMele != null)
        {
            firstMele.SetActive(state == 2);
        }
    }
}
