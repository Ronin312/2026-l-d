using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostGame.Environment;

namespace FrostGame.Enemies
{
    /// <summary>
    /// Cienisty Łucznik: utrzymuje dystans, strzela z zapowiedzią (linia celowania),
    /// a gdy gracz podejdzie zbyt blisko, robi unik w tył i teleportuje się w najdalszy punkt.
    /// </summary>
    public class ShadowArcher : EnemyBaseWithAnim
    {
        [SerializeField] ShadowArcherEnemyData shadowArcherEnemyData;
        
        [Header("Strzał")] [SerializeField] EnemyProjectile projectilePrefab;
        [SerializeField] Transform firePoint;
        [SerializeField] LineRenderer aimLine;
        [SerializeField] float aimLineLength = 12f;
        [SerializeField] float aimLockTime = 0.25f; // ostatni moment, gdy celownik jest już zablokowany
        [SerializeField] LayerMask obstacleMask;

        [Header("Cover & Movement")]
        [SerializeField] List<HidingSpot> hidingSpots = new List<HidingSpot>();
        [SerializeField] float sprintSpeed = 10f;
        [SerializeField] float stunDuration = 2f;

        HidingSpot currentSpot;
        float nextShotTime;
        bool isAiming;
        bool isStunned;
        bool isRelocating;
        Coroutine aimRoutine;
        Coroutine stunRoutine;
        HidingSpot targetCoverSpot; float relocateTimeout;
        
        protected override void Start()
        {
            base.Start();
            if (aimLine != null) aimLine.enabled = false;
            if (firePoint == null) firePoint = transform;

            // Auto-gather spots in the scene if list is empty
            if (hidingSpots.Count == 0)
            {
                hidingSpots.AddRange(FindObjectsOfType<HidingSpot>());
            }
        }
        
        protected override void Tick()
        {
            if (target != null)
                sprite.flipX = DirectionToTarget.x <= 0;
            
            if (currentSpot != null && (currentSpot.IsDestroyed || !currentSpot.gameObject.activeInHierarchy)) 
                OnCoverLost();
            
            //Stunned or moving to cover
            if (isStunned) return;

            if (isRelocating && targetCoverSpot != null)
            { 
                anim.Play("Move"); 
                Vector2 dir = ((Vector2)targetCoverSpot.transform.position - (Vector2)transform.position).normalized; 
                Move(dir, sprintSpeed); // Check if we arrived at the spot
                sprite.flipX = dir.x <= 0;
                
                if (Vector2.Distance(transform.position, targetCoverSpot.transform.position) <= 0.5f)
                {
                    EnterCover(targetCoverSpot);
                }
                else if (Time.time >= relocateTimeout)
                {
                    targetCoverSpot.SetOccupied(false); 
                    targetCoverSpot = null; 
                    isRelocating = false; 
                    SetVisibility(true);
                }
                return; 
            }
            //Out of cover
            if (currentSpot == null)
            {
                HidingSpot nextSpot = FindFarthestSpot();
                if (nextSpot != null)
                {
                    StartRelocating(nextSpot); 
                    return;
                }
            }

            //If in cover, stay hidden; if no cover exists, stand still and play normal animations
            if (currentSpot != null)
            {
                if (sprite.enabled && !isAiming) SetVisibility(false);
            }
            else
            {
                if (!sprite.enabled) SetVisibility(true);
                if (!isAiming) anim.Play("Idle");
            }

            //Shooting
            if (target != null && DistanceToTarget <= data.detectionRange && !isAiming && Time.time >= nextShotTime && HasLineOfSight())
            {
                aimRoutine = StartCoroutine(AimAndShoot());
            }
        }

        public void OnCoverLost()
        {
            if (currentSpot != null)
            {
                currentSpot.SetOccupied(false); 
                currentSpot = null;
            } 
            CancelAim(); 
            SetVisibility(true);
        }
        
        bool HasLineOfSight()
        {
            return !Physics2D.Linecast(firePoint.position+offset, target.position, obstacleMask);
        }

        IEnumerator AimAndShoot()
        {
            isAiming = true;
            if (aimLine != null) aimLine.enabled = true;
            if (currentSpot == null)
            {
                anim.Play("Attack");
            }
            Vector2 lockedDir = DirectionToTarget;
            float t = 0f;
            while (t < shadowArcherEnemyData.aimTime)
            {
                if (target == null || isStunned || isRelocating)
                {
                    CancelAim();
                    yield break;
                }
                Vector2 origin = firePoint.position + offset;
                if (t < shadowArcherEnemyData.aimTime - aimLockTime)
                {
                    lockedDir = ((Vector2)target.position - origin).normalized;
                }
                else if (currentSpot != null)
                {
                    // Brief reveal
                    SetVisibility(true);
                    anim.Play("Attack");
                }
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
                var proj = Instantiate(projectilePrefab, firePoint.position + offset, Quaternion.identity);
                proj.Init(lockedDir, shadowArcherEnemyData.projectileDamage, shadowArcherEnemyData.projectileSpeed);
            }
            if (currentSpot != null)
            {
                SetVisibility(false);
            }
            nextShotTime = Time.time + shadowArcherEnemyData.shotCooldown;
            isAiming = false;
            aimRoutine = null;
        }
        
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            
            if (isActiveAndEnabled)
            {
                isRelocating = false;
                if (stunRoutine != null) 
                    StopCoroutine(stunRoutine); 
                stunRoutine = StartCoroutine(StunRoutine());
            }
        }
        
        void CancelAim()
        {
            if (aimRoutine != null) StopCoroutine(aimRoutine);
            aimRoutine = null;
            isAiming = false;
            if (aimLine != null) aimLine.enabled = false;
        }

        IEnumerator StunRoutine()
        {
            isStunned = true;
            CancelAim();
            SetVisibility(true);

            if (targetCoverSpot != null)
            {
                targetCoverSpot.SetOccupied(false); 
                targetCoverSpot = null;
            }
            isRelocating = false;
            
            if (currentSpot != null)
            {
                currentSpot.SetOccupied(false);
                currentSpot = null;
            }
            
            anim.Play("Hurt");
            yield return new WaitForSeconds(stunDuration);

            isStunned = false;
        }
        
        HidingSpot FindFarthestSpot()
        {
            HidingSpot best = null;
            float maxDist = float.MinValue;

            foreach (var spot in hidingSpots)
            {
                if (spot == null || !spot.gameObject.activeInHierarchy || spot.IsOccupied || spot.IsDestroyed)
                    continue;
                
                float score;
                if (target != null)
                {
                    score = Vector2.Distance(spot.transform.position, target.position);
                }
                else
                {
                    score = -Vector2.Distance(transform.position, spot.transform.position);
                }

                if (score > maxDist || best == null)
                {
                    maxDist = score;
                    best = spot;
                }
            }
            return best;
        }

   
        void StartRelocating(HidingSpot spot) 
        { 
            targetCoverSpot = spot; 
            targetCoverSpot.SetOccupied(true); 
            isRelocating = true; 
            SetVisibility(true); 
            relocateTimeout = Time.time + 4.0f; // 4 second safety failsafe
        }

        void EnterCover(HidingSpot spot)
        {
            currentSpot = spot; 
            targetCoverSpot = null; 
            isRelocating = false; 
            SetVisibility(false);
        }
        
        void SetVisibility(bool isVisible)
        {
            if (sprite != null) sprite.enabled = isVisible;
        }

        protected override void Die()
        {
            CancelAim();
            if (currentSpot != null)
            {
                currentSpot.SetOccupied(false); 
                currentSpot = null;
            } 
            SetVisibility(true); 
            base.Die();
        }

        void OnDrawGizmosSelected()
        {
            if (shadowArcherEnemyData == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + offset, shadowArcherEnemyData.preferredDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + offset, shadowArcherEnemyData.panicDistance);
        }
    }
}