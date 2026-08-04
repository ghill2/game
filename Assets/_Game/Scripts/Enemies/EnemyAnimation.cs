using UnityEngine;
using UnityEngine.AI;

public sealed class EnemyAnimation : MonoBehaviour
{
    private static readonly int SpeedParameter =
        Animator.StringToHash("Speed");

    [SerializeField]
    private NavMeshAgent agent;
    
    [SerializeField]
    private Animator animator;

    [Tooltip("Makes the animation change more smoothly.")]
    [SerializeField, Min(0f)]
    private float speedDamping = 0.1f;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        float normalizedSpeed = 0f;

        if (agent != null &&
            agent.enabled &&
            agent.isOnNavMesh &&
            agent.speed > 0.01f)
        {
            normalizedSpeed =
                agent.velocity.magnitude / agent.speed;
        }

        animator.SetFloat(
            SpeedParameter,
            normalizedSpeed,
            speedDamping,
            Time.deltaTime);
    }
}