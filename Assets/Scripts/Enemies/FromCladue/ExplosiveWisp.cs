using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ognik Wybuchowy: kamikaze. Leci falistym torem do gracza, w zasięgu zaczyna migać i wybucha.
/// Zabity wcześniej eksploduje od razu. Wybuch rani gracza i innych wrogów (też inne ognie,
/// co daje reakcje łańcuchowe), więc można go odciągnąć w tłum.
/// Explosion Mask musi obejmować warstwę gracza i warstwę wrogów.
/// </summary>
public class ExplosiveWisp : EnemyBase
{
    enum State { Seek, Fuse }

    [SerializeField] LayerMask explosionMask;
    [Tooltip("Efekt wybuchu (powinien sam się niszczyć). Skalowany do średnicy wybuchu.")]
    [SerializeField] GameObject blastVfx;
    [SerializeField] float bobAmplitude = 0.1f;
    [SerializeField] float blinkFrequency = 30f;

    State state = State.Seek;
    float fuseEnd;
    float phase;
    bool exploded;
    Vector3 spriteBaseLocalPos;

    protected override void Awake()
    {
        base.Awake();
        phase = Random.value * Mathf.PI * 2f;
        if (sprite != null) spriteBaseLocalPos = sprite.transform.localPosition;
    }

    protected override void Tick()
    {
        // Falowanie sprite'a (tylko gdy sprite jest na obiekcie podrzędnym, żeby nie ruszać korzenia).
        if (sprite != null && sprite.transform != transform)
        {
            sprite.transform.localPosition =
                spriteBaseLocalPos + Vector3.up * Mathf.Sin(Time.time * 4f + phase) * bobAmplitude;
        }

        if (target == null) return;

        if (state == State.Seek)
        {
            float dist = DistanceToTarget;
            if (dist > data.detectionRange) return;

            if (dist <= data.wispTriggerRange)
            {
                state = State.Fuse;
                fuseEnd = Time.time + data.wispFuseTime;
                return;
            }

            Vector2 dir = DirectionToTarget;
            Vector2 wobble = Vector2.Perpendicular(dir) *
                             (Mathf.Sin(Time.time * data.wispWobbleFrequency + phase) * data.wispWobble);
            Move(dir + wobble, data.moveSpeed);
        }
        else
        {
            SetTint(Mathf.Sin(Time.time * blinkFrequency) > 0f ? Color.red : Color.white);
            if (Time.time >= fuseEnd) Die();
        }
    }

    protected override void Die()
    {
        if (isDead) return;

        if (!exploded)
        {
            exploded = true;
            Blast();
        }

        base.Die();
    }

    void Blast()
    {
        var hits = Physics2D.OverlapCircleAll(rb.position, data.wispBlastRadius, explosionMask);
        var damaged = new HashSet<IDamageable>();

        foreach (var hit in hits)
        {
            var damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null || ReferenceEquals(damageable, this) || !damaged.Add(damageable)) continue;
            damageable.TakeDamage(data.wispBlastDamage);
        }

        if (blastVfx != null)
        {
            var vfx = Instantiate(blastVfx, transform.position, Quaternion.identity);
            vfx.transform.localScale = Vector3.one * data.wispBlastRadius * 2f;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.wispBlastRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.wispTriggerRange);
    }
}
