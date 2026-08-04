using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyAttack : MonoBehaviour
{
    private static readonly int AttackTrigger =
        Animator.StringToHash("Attack");

    [Header("Attack position")]
    [SerializeField] private Transform attackOrigin;

    [Header("Target filter")]
    [SerializeField] private LayerMask playerLayers = ~0;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Tooltip(
        "Enable only after OpenAttackWindow and " +
        "CloseAttackWindow events are added to the clip.")]
    [SerializeField] private bool useAnimationEvents;

    private readonly Collider[] hitBuffer =
        new Collider[8];

    private readonly HashSet<IDamageable> damagedTargets =
        new HashSet<IDamageable>();

    private EnemyData data;
    private Transform currentTarget;
    private Coroutine fallbackRoutine;

    private bool isAttacking;
    private bool attackWindowOpen;
    private float nextAttackTime;

    public bool IsAttacking => isAttacking;
    public bool IsAttackWindowOpen => attackWindowOpen;

    public void Configure(EnemyData enemyData)
    {
        data = enemyData;

        if (attackOrigin == null)
        {
            attackOrigin = transform;
        }
    }

    private void Update()
    {
        if (attackWindowOpen)
        {
            DamageTargetsInRange();
        }
    }

    public bool TryStartAttack(Transform target)
    {
        if (data == null ||
            target == null ||
            isAttacking ||
            Time.time < nextAttackTime)
        {
            return false;
        }

        currentTarget = target;
        isAttacking = true;
        attackWindowOpen = false;

        damagedTargets.Clear();

        nextAttackTime =
            Time.time + data.attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger(AttackTrigger);
        }

        if (useAnimationEvents && animator != null)
        {
            return true;
        }

        fallbackRoutine =
            StartCoroutine(TimedAttackRoutine());

        return true;
    }

    public void OpenAttackWindow()
    {
        if (!isAttacking || attackWindowOpen)
        {
            return;
        }

        attackWindowOpen = true;
    }

    public void CloseAttackWindow()
    {
        if (!isAttacking)
        {
            return;
        }

        attackWindowOpen = false;
        isAttacking = false;
        currentTarget = null;
    }

    public void CancelAttack()
    {
        if (fallbackRoutine != null)
        {
            StopCoroutine(fallbackRoutine);
            fallbackRoutine = null;
        }

        if (animator != null)
        {
            animator.ResetTrigger(AttackTrigger);
        }

        attackWindowOpen = false;
        isAttacking = false;
        currentTarget = null;

        damagedTargets.Clear();
    }

    private IEnumerator TimedAttackRoutine()
    {
        yield return new WaitForSeconds(
            data.attackWindupDuration);

        OpenAttackWindow();

        yield return new WaitForSeconds(
            data.attackWindowDuration);

        CloseAttackWindow();
        fallbackRoutine = null;
    }

    private void DamageTargetsInRange()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            attackOrigin.position,
            data.attackHitRadius,
            hitBuffer,
            playerLayers,
            QueryTriggerInteraction.Collide);

        for (int index = 0; index < hitCount; index++)
        {
            Collider hitCollider = hitBuffer[index];

            if (hitCollider == null)
            {
                continue;
            }

            IDamageable damageable =
                hitCollider.GetComponentInParent<IDamageable>();

            if (damageable == null ||
                !damageable.IsAlive ||
                damagedTargets.Contains(damageable))
            {
                continue;
            }

            Component damageableComponent =
                damageable as Component;

            if (damageableComponent == null ||
                currentTarget == null ||
                damageableComponent.transform.root !=
                currentTarget.root)
            {
                continue;
            }

            // Add before applying damage to prevent repeat calls.
            damagedTargets.Add(damageable);
            damageable.TakeDamage(data.attackDamage);
        }
    }

    private void OnDisable()
    {
        CancelAttack();
    }
}