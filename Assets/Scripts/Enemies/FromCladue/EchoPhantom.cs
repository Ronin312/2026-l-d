using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Echo: duch, który odtwarza trasę gracza z opóźnieniem. Przenika ściany (collider jako trigger),
/// rani przy dotknięciu. Trafienie go na chwilę rozprasza (staje się przezroczysty i nieszkodliwy),
/// a potem zaczyna zapisywać trasę od nowa.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EchoPhantom : EnemyBase
{
    [SerializeField, Range(0f, 1f)] float disperseAlpha = 0.25f;

    readonly Queue<Vector2> history = new Queue<Vector2>();
    int delaySamples;
    float sampleTimer;
    Vector2 followPoint;
    bool hasPoint;
    bool dispersed;
    float disperseEnd;
    float nextHitTime;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Collider2D>().isTrigger = true;
        delaySamples = Mathf.Max(1, Mathf.RoundToInt(data.echoDelay / data.echoSampleInterval));
    }

    protected override void Tick()
    {
        if (target == null) return;

        if (dispersed)
        {
            if (Time.time >= disperseEnd)
            {
                dispersed = false;
                hasPoint = false;
                history.Clear();
                SetTint(Color.white);
            }
            return;
        }

        sampleTimer += Time.deltaTime;
        while (sampleTimer >= data.echoSampleInterval)
        {
            sampleTimer -= data.echoSampleInterval;
            history.Enqueue(target.position);

            if (history.Count > delaySamples)
            {
                followPoint = history.Dequeue();
                hasPoint = true;
            }
        }

        if (!hasPoint) return;

        Vector2 delta = followPoint - rb.position;
        if (delta.magnitude > 0.05f)
            Move(delta, Mathf.Min(delta.magnitude * 10f, data.echoSpeed * 2f));
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDead || dispersed || Time.time < nextHitTime) return;
        if (!other.CompareTag("Player")) return;

        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null) return;

        damageable.TakeDamage(data.contactDamage);
        nextHitTime = Time.time + data.contactCooldown;
    }

    public override void TakeDamage(int amount)
    {
        if (isDead) return;

        base.TakeDamage(amount);
        if (!isDead) Disperse();
    }

    void Disperse()
    {
        dispersed = true;
        disperseEnd = Time.time + data.echoDisperseTime;
        SetTint(new Color(1f, 1f, 1f, disperseAlpha));
    }
}
