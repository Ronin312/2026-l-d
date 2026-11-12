using UnityEngine;

/// <summary>
/// Tarcza nakładana przez Szamana Run. Pochłania kilka trafień, zanim wróg zacznie tracić HP.
/// Dodawana w locie przez ShieldBuff.Apply(), więc nie trzeba jej ręcznie dodawać do prefabów.
/// </summary>
public class ShieldBuff : MonoBehaviour
{
    int hitsLeft;
    LineRenderer ring;

    public int HitsLeft => hitsLeft;

    public static ShieldBuff Apply(GameObject target, int hits)
    {
        var shield = target.GetComponent<ShieldBuff>();
        if (shield == null)
        {
            shield = target.AddComponent<ShieldBuff>();

            float radius = 0.8f;
            var col = target.GetComponent<Collider2D>();
            if (col != null)
                radius = Mathf.Max(col.bounds.extents.x, col.bounds.extents.y) + 0.15f;
            shield.BuildRing(radius);
        }

        shield.hitsLeft = hits;
        return shield;
    }

    /// <summary>Zwraca true, jeśli tarcza pochłonęła trafienie.</summary>
    public bool Absorb()
    {
        if (hitsLeft <= 0) return false;
        hitsLeft--;
        if (hitsLeft <= 0) Remove();
        return true;
    }

    public void Remove()
    {
        hitsLeft = 0;
        Destroy(this);
    }

    void OnDestroy()
    {
        if (ring != null) Destroy(ring.gameObject);
    }

    void BuildRing(float worldRadius)
    {
        var go = new GameObject("ShieldRing");
        go.transform.SetParent(transform, false);

        ring = go.AddComponent<LineRenderer>();
        ring.useWorldSpace = false;
        ring.loop = true;
        ring.material = new Material(Shader.Find("Sprites/Default"));
        ring.startColor = ring.endColor = new Color(0.4f, 0.9f, 1f, 0.9f);
        ring.sortingOrder = 20;

        const int points = 28;
        float scale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), 0.0001f);
        float r = worldRadius / scale;
        ring.widthMultiplier = 0.06f / scale;
        ring.positionCount = points;
        for (int i = 0; i < points; i++)
        {
            float a = i * Mathf.PI * 2f / points;
            ring.SetPosition(i, new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r, 0f));
        }
    }
}
