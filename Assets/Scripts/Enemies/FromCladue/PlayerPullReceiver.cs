using System.Collections;
using UnityEngine;

/// <summary>
/// Przykładowa implementacja IPullable dla gracza (Rigidbody2D). Dodaj do obiektu gracza.
/// Twój kontroler ruchu powinien ignorować wejście, gdy IsBeingPulled jest true.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPullReceiver : MonoBehaviour, IPullable
{
    Rigidbody2D rb;
    Coroutine routine;

    public bool IsBeingPulled { get; private set; }

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void ApplyPull(Transform anchor, float speed, float duration, float stopDistance)
    {
        CancelPull();
        routine = StartCoroutine(Pull(anchor, speed, duration, stopDistance));
    }

    public void CancelPull()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
        IsBeingPulled = false;
    }

    IEnumerator Pull(Transform anchor, float speed, float duration, float stopDistance)
    {
        IsBeingPulled = true;
        float t = 0f;

        while (t < duration && anchor != null)
        {
            Vector2 toAnchor = (Vector2)anchor.position - rb.position;
            float dist = toAnchor.magnitude;
            if (dist <= stopDistance) break;

            float step = Mathf.Min(speed * Time.fixedDeltaTime, dist - stopDistance);
            rb.MovePosition(rb.position + toAnchor.normalized * step);

            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        IsBeingPulled = false;
        routine = null;
    }
}
