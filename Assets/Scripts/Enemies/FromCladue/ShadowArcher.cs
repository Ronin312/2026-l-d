using System.Collections;
using UnityEngine;

/// <summary>
/// Cienisty Łucznik: utrzymuje dystans, strzela z zapowiedzią (linia celowania),
/// a gdy gracz podejdzie zbyt blisko, robi unik w tył i teleportuje się w najdalszy punkt.
/// </summary>
public class ShadowArcher : EnemyBase
{
    [Header("Strzał")]
    [SerializeField] EnemyProjectile projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] LineRenderer aimLine;
    [SerializeField] float aimLineLength = 12f;
    [SerializeField] float aimLockTime = 0.25f; // ostatni moment, gdy celownik jest już zablokowany
    [SerializeField] LayerMask obstacleMask;

    [Header("Unik i teleport")]
    [SerializeField] Transform[] teleportPoints;
    [SerializeField] float dodgeDuration = 0.15f;
    [SerializeField] float dodgeSpeed = 12f;
    [SerializeField] GameObject teleportVfx;

    float nextShotTime;
    float nextTeleportTime;
    bool isAiming;
    bool isBusy;
    Coroutine aimRoutine;

    protected override void Start()
    {
        base.Start();
        if (aimLine != null) aimLine.enabled = false;
        if (firePoint == null) firePoint = transform;
    }

    protected override void Tick()
    {
        if (target == null || isBusy) return;

        float dist = DistanceToTarget;
        if (dist > data.detectionRange) return;

        bool panic = dist < data.panicDistance;
        if (panic && Time.time >= nextTeleportTime && teleportPoints != null && teleportPoints.Length > 0)
        {
            StartCoroutine(DodgeAndTeleport());
            return;
        }

        if (!isAiming)
        {
            if (dist < data.preferredDistance - data.distanceTolerance)
                Move(-DirectionToTarget, data.moveSpeed);
            else if (dist > data.preferredDistance + data.distanceTolerance)
                Move(DirectionToTarget, data.moveSpeed * 0.7f);
        }

        if (!isAiming && Time.time >= nextShotTime && HasLineOfSight())
            aimRoutine = StartCoroutine(AimAndShoot());
    }

    bool HasLineOfSight()
    {
        return !Physics2D.Linecast(firePoint.position, target.position, obstacleMask);
    }

    IEnumerator AimAndShoot()
    {
        isAiming = true;
        if (aimLine != null) aimLine.enabled = true;

        Vector2 lockedDir = DirectionToTarget;
        float t = 0f;

        while (t < data.aimTime)
        {
            if (target == null) { CancelAim(); yield break; }

            Vector2 origin = firePoint.position;
            if (t < data.aimTime - aimLockTime)
                lockedDir = ((Vector2)target.position - origin).normalized;

            if (aimLine != null)
            {
                aimLine.SetPosition(0, origin);
                aimLine.SetPosition(1, origin + lockedDir * aimLineLength);
            }

            t += Time.deltaTime;
            yield return null;
        }

        if (aimLine != null) aimLine.enabled = false;

        if (projectilePrefab != null)
        {
            var projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            projectile.Init(lockedDir, data.projectileDamage, data.projectileSpeed);
        }

        nextShotTime = Time.time + data.shotCooldown;
        isAiming = false;
        aimRoutine = null;
    }

    void CancelAim()
    {
        if (aimRoutine != null) StopCoroutine(aimRoutine);
        aimRoutine = null;
        isAiming = false;
        if (aimLine != null) aimLine.enabled = false;
    }

    IEnumerator DodgeAndTeleport()
    {
        isBusy = true;
        CancelAim();

        Vector2 away = -DirectionToTarget;
        float t = 0f;
        while (t < dodgeDuration)
        {
            rb.MovePosition(rb.position + away * dodgeSpeed * Time.fixedDeltaTime);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Teleport();
        nextTeleportTime = Time.time + data.teleportCooldown;
        isBusy = false;
    }

    void Teleport()
    {
        if (target == null) return;

        Transform best = null;
        float bestDist = -1f;
        foreach (var point in teleportPoints)
        {
            float d = Vector2.Distance(point.position, target.position);
            if (d > bestDist) { bestDist = d; best = point; }
        }
        if (best == null) return;

        if (teleportVfx != null) Instantiate(teleportVfx, transform.position, Quaternion.identity);
        rb.position = best.position;
        transform.position = best.position;
        if (teleportVfx != null) Instantiate(teleportVfx, best.position, Quaternion.identity);
    }

    protected override void Die()
    {
        CancelAim();
        base.Die();
    }

    void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, data.preferredDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.panicDistance);
    }
}
