using UnityEngine;

/// <summary>
/// Duch przywoływany przez Szamana Run. Szybko goni gracza i znika po określonym czasie.
/// Używa własnego EnemyData (np. maxHealth 1, contactDamage 1, moveSpeed 3.5).
/// </summary>
public class SpiritMinion : EnemyBase
{
    float dieAt = float.MaxValue;

    public void Init(float lifetime)
    {
        dieAt = Time.time + lifetime;
    }

    protected override void Tick()
    {
        if (Time.time >= dieAt)
        {
            Die();
            return;
        }

        if (target != null)
            Move(DirectionToTarget, data.moveSpeed);
    }
}
