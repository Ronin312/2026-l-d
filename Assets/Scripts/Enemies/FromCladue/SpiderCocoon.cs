using UnityEngine;

/// <summary>
/// Kokon po zabitym pająku. Po czasie wykluwa nowego pająka, chyba że gracz go zniszczy.
/// Wymaga Collider2D, żeby atak gracza mógł go trafić.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SpiderCocoon : MonoBehaviour, IDamageable
{
    SwarmManager manager;
    float hatchAt;
    bool initialized;

    public void Init(SwarmManager owner, float hatchTime)
    {
        manager = owner;
        hatchAt = Time.time + hatchTime;
        initialized = true;
    }

    void Update()
    {
        if (!initialized || Time.time < hatchAt) return;

        if (manager != null) manager.SpawnSpider(transform.position);
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        Destroy(gameObject);
    }
}
