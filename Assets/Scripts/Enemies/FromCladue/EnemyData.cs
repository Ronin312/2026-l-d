using UnityEngine;

/// <summary>
/// Parametry wroga. Utwórz po jednym assecie na typ: Create > Enemies > Enemy Data.
/// Każdy typ używa tylko swojej sekcji (reszta jest ignorowana).
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Wspólne")]
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public int contactDamage = 1;
    public float contactCooldown = 0.8f;
    public float detectionRange = 8f;

    [Header("Kamienny Strażnik")]
    public int armorHealth = 15;
    public float attackRange = 1.8f;
    public float windupTime = 0.7f;
    public float slamCooldown = 2f;
    public int slamDamage = 2;
    public float slamRadius = 1.4f;
    public float stunDuration = 2f;

    [Header("Cienisty Łucznik")]
    public float preferredDistance = 6f;
    public float distanceTolerance = 1f;
    public float panicDistance = 3f;
    public float shotCooldown = 2.5f;
    public float aimTime = 0.8f;
    public float projectileSpeed = 8f;
    public int projectileDamage = 1;
    public float teleportCooldown = 5f;

    [Header("Rojowy Pająk")]
    public float chargeSpeedMultiplier = 2.2f;
    public float cocoonHatchTime = 3f;

    [Header("Szaman Run")]
    public float shamanKeepDistance = 7f;
    public float shieldInterval = 4f;
    public int shieldHits = 3;
    public float shieldRange = 8f;
    public float summonCooldown = 6f;
    public int summonCount = 2;
    public float spiritLifetime = 6f;

    [Header("Rogaty Byk")]
    public float bullChargeRange = 7f;
    public float bullWindupTime = 1f;
    public float bullChargeSpeed = 10f;
    public int bullChargeDamage = 3;
    public float bullMaxChargeTime = 1.2f;
    public float bullChargeCooldown = 2f;
    public float bullStunDuration = 2.5f;
    public float bullStunDamageMultiplier = 2f;

    [Header("Kret Zasadzkowiec")]
    public float moleTelegraphTime = 1f;
    public float moleTriggerDistance = 0.6f;
    public float moleBurstRadius = 1.6f;
    public int moleBurstDamage = 2;
    public float moleSurfacedTime = 3f;
    public float moleUndergroundMaxTime = 6f;

    [Header("Echo")]
    public float echoDelay = 2f;
    public float echoSampleInterval = 0.1f;
    public float echoDisperseTime = 1.2f;
    public float echoSpeed = 6f;

    [Header("Łowca z Łańcuchem")]
    public float hookKeepDistance = 6f;
    public float hookRange = 10f;
    public float hookTelegraphTime = 0.6f;
    public float hookSpeed = 18f;
    public float hookCooldown = 4f;
    public int hookDamage = 1;
    public float hookPullSpeed = 14f;
    public float hookPullDuration = 0.5f;
    public float hookStopDistance = 1.2f;
    public float hookBlockedStunTime = 2f;
    public float hookStunDamageMultiplier = 2f;

    [Header("Ognik Wybuchowy")]
    public float wispTriggerRange = 1.8f;
    public float wispFuseTime = 0.8f;
    public float wispBlastRadius = 2.2f;
    public int wispBlastDamage = 3;
    public float wispWobble = 0.6f;
    public float wispWobbleFrequency = 3f;
}
