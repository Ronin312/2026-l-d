using UnityEngine;

/// <summary>
/// Rogaty Byk: szarżownik. Gdy ma linię widzenia i gracz jest w zasięgu, grzebie kopytem (zapowiedź),
/// po czym szarżuje w prostej linii. Uderzenie w przeszkodę ogłusza go i zwiększa otrzymywane obrażenia.
/// Warstwa Byka NIE może wchodzić w Obstacle Mask, inaczej będzie wykrywał sam siebie.
/// </summary>
public class HornedBull : EnemyBase
{
    enum State { Idle, Windup, Charge, Recover, Stunned }

    [Header("Szarża")]
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float wallCheckDistance = 0.7f;
    [SerializeField] float hitRadius = 0.7f;
    [SerializeField] float recoverTime = 0.5f;
    [SerializeField] float aimLockTime = 0.25f;

    [Header("Kolory (feedback)")]
    [SerializeField] Color windupColor = new Color(1f, 0.5f, 0.3f);
    [SerializeField] Color stunColor = new Color(1f, 1f, 0.4f);

    State state = State.Idle;
    float stateEnd;
    float chargeEnd;
    float nextChargeTime;
    Vector2 chargeDir;

    protected override void Tick()
    {
        switch (state)
        {
            case State.Idle: TickIdle(); break;
            case State.Windup: TickWindup(); break;
            case State.Charge: TickCharge(); break;
            case State.Recover:
                if (Time.time >= stateEnd) state = State.Idle;
                break;
            case State.Stunned:
                if (Time.time >= stateEnd)
                {
                    state = State.Idle;
                    SetTint(Color.white);
                }
                break;
        }
    }

    void TickIdle()
    {
        if (target == null || DistanceToTarget > data.detectionRange) return;

        if (DistanceToTarget <= data.bullChargeRange && Time.time >= nextChargeTime && HasLineOfSight())
        {
            state = State.Windup;
            stateEnd = Time.time + data.bullWindupTime;
            chargeDir = DirectionToTarget;
            SetTint(windupColor);
            return;
        }

        Move(DirectionToTarget, data.moveSpeed);
    }

    void TickWindup()
    {
        if (target != null && Time.time < stateEnd - aimLockTime)
            chargeDir = DirectionToTarget; // celuje do ostatniej chwili, potem kierunek jest zablokowany

        if (Time.time < stateEnd) return;

        state = State.Charge;
        chargeEnd = Time.time + data.bullMaxChargeTime;
        SetTint(Color.white);
    }

    void TickCharge()
    {
        if (Physics2D.Raycast(rb.position, chargeDir, wallCheckDistance, obstacleMask))
        {
            CrashIntoWall();
            return;
        }

        var hit = Physics2D.OverlapCircle(rb.position, hitRadius, playerLayer);
        if (hit != null)
        {
            var damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null) damageable.TakeDamage(data.bullChargeDamage);
            EndCharge();
            return;
        }

        if (Time.time >= chargeEnd)
        {
            EndCharge();
            return;
        }

        Move(chargeDir, data.bullChargeSpeed);
    }

    void EndCharge()
    {
        state = State.Recover;
        stateEnd = Time.time + recoverTime;
        nextChargeTime = Time.time + data.bullChargeCooldown;
    }

    void CrashIntoWall()
    {
        state = State.Stunned;
        stateEnd = Time.time + data.bullStunDuration;
        nextChargeTime = stateEnd + data.bullChargeCooldown;
        SetTint(stunColor);
    }

    bool HasLineOfSight()
    {
        return !Physics2D.Linecast(rb.position, target.position, obstacleMask);
    }

    public override void TakeDamage(int amount)
    {
        if (isDead) return;

        if (state == State.Stunned)
            amount = Mathf.RoundToInt(amount * data.bullStunDamageMultiplier);

        base.TakeDamage(amount);
    }

    void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.bullChargeRange);
    }
}
