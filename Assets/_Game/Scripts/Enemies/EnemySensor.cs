using UnityEngine;

public sealed class EnemySensor : MonoBehaviour
{
    [SerializeField] private Transform target;

    private IDamageable targetDamageable;

    public Transform Target => target;

    public bool HasActiveTarget =>
        target != null &&
        (targetDamageable == null || targetDamageable.IsAlive);

    public void Initialize()
    {
        FindPlayerIfNeeded();
        CacheDamageable();
    }

    public void FindPlayerIfNeeded()
    {
        if (target != null)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
            CacheDamageable();
        }
    }

    public bool CanDetect(
        Vector3 guardPoint,
        float detectionRange,
        float encounterRadius)
    {
        if (!HasActiveTarget)
        {
            return false;
        }

        bool closeEnough =
            PlanarSqrDistance(transform.position, target.position) <=
            detectionRange * detectionRange;

        return closeEnough &&
               IsInsideEncounter(guardPoint, encounterRadius);
    }

    public bool IsInsideEncounter(
        Vector3 guardPoint,
        float encounterRadius)
    {
        if (!HasActiveTarget)
        {
            return false;
        }

        return PlanarSqrDistance(guardPoint, target.position) <=
               encounterRadius * encounterRadius;
    }

    public bool IsInsideAttackRange(float attackRange)
    {
        if (!HasActiveTarget)
        {
            return false;
        }

        return PlanarSqrDistance(transform.position, target.position) <=
               attackRange * attackRange;
    }

    private void CacheDamageable()
    {
        targetDamageable = target != null
            ? target.GetComponentInParent<IDamageable>()
            : null;
    }

    private static float PlanarSqrDistance(
        Vector3 first,
        Vector3 second)
    {
        first.y = 0f;
        second.y = 0f;

        return (first - second).sqrMagnitude;
    }
}