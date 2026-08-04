using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public sealed class EnemyMotor : MonoBehaviour
{
    private NavMeshAgent agent;

    public bool IsReady =>
        agent != null &&
        agent.enabled &&
        agent.isOnNavMesh;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Configure(EnemyData data)
    {
        agent.speed = data.moveSpeed;
        agent.angularSpeed = data.angularSpeed;
        agent.acceleration = data.moveSpeed * 4f;
        agent.autoBraking = true;
    }

    public void MoveTo(Vector3 destination, float stoppingDistance)
    {
        if (!IsReady)
        {
            return;
        }

        agent.stoppingDistance = stoppingDistance;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    public void Stop()
    {
        if (!IsReady)
        {
            return;
        }

        agent.isStopped = true;
        agent.ResetPath();
    }

    public bool HasReachedDestination(float tolerance = 0.15f)
    {
        if (!IsReady || agent.pathPending)
        {
            return false;
        }

        float allowedDistance =
            Mathf.Max(agent.stoppingDistance, tolerance);

        return agent.remainingDistance <= allowedDistance &&
               agent.velocity.sqrMagnitude < 0.01f;
    }

    public void Face(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            agent.angularSpeed * Time.deltaTime);
    }

    public void DisableMovement()
    {
        if (agent == null)
        {
            return;
        }

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        agent.enabled = false;
    }
}