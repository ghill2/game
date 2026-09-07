using System;
using UnityEngine;

[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(EnemyMotor))]
[RequireComponent(typeof(EnemyAttack))]
[RequireComponent(typeof(EnemyHealth))]
public sealed class EnemyBrain : MonoBehaviour
{
    public enum EnemyState
    {
        Guard = 0,
        Chase = 1,
        Attack = 2,
        Disengage = 3,
        Hurt = 4,
        Defeated = 5
    }

    [SerializeField] private EnemyData data;
    [SerializeField] private NpcType npcType;
    [SerializeField] private EnemyState currentState;
    [SerializeField] private bool logStateChanges = true;
    private bool reportedCombatEngagement;

    public static event Action<EnemyBrain, bool>
        CombatEngagementChanged;

    private EnemySensor sensor;
    private EnemyMotor motor;
    private EnemyAttack attack;
    private EnemyHealth health;

    private Vector3 guardPoint;
    private float nextRepathTime;
    private float hurtEndTime;

    public EnemyState CurrentState => currentState;
    public bool IsEngagedInCombat =>
        isActiveAndEnabled && IsCombatState(currentState);

    private void Awake()
    {
        sensor = GetComponent<EnemySensor>();
        motor = GetComponent<EnemyMotor>();
        attack = GetComponent<EnemyAttack>();
        health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        health.Damaged += HandleDamaged;
        health.Defeated += HandleDefeated;
        UpdateCombatEngagement();
    }

    private void OnDisable()
    {
        health.Damaged -= HandleDamaged;
        health.Defeated -= HandleDefeated;
        SetCombatEngagement(false);
    }

    private void Start()
    {
        if (data == null)
        {
            Debug.LogError(
                $"{name} has no EnemyData assigned.",
                this);

            enabled = false;
            return;
        }

        guardPoint = transform.position;

        health.Configure(data.maximumHealth);
        sensor.Initialize();
        motor.Configure(data);

        if (!motor.IsReady)
        {
            Debug.LogWarning(
                $"{name} is not placed on a baked NavMesh.",
                this);
        }

        attack.Configure(data);

        SetState(EnemyState.Guard, true);
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Guard:
                UpdateGuard();
                break;

            case EnemyState.Chase:
                UpdateChase();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;

            case EnemyState.Disengage:
                UpdateDisengage();
                break;

            case EnemyState.Hurt:
                UpdateHurt();
                break;

            case EnemyState.Defeated:
                break;
        }
    }

    private void UpdateGuard()
    {
        sensor.FindPlayerIfNeeded();

        if (sensor.CanDetect(
                guardPoint,
                data.detectionRange,
                data.encounterRadius))
        {
            SetState(EnemyState.Chase);
        }
    }

    private void UpdateChase()
    {
        if (!sensor.IsInsideEncounter(
                guardPoint,
                data.encounterRadius))
        {
            SetState(EnemyState.Disengage);
            return;
        }

        if (sensor.IsInsideAttackRange(data.attackRange))
        {
            SetState(EnemyState.Attack);
            return;
        }

        if (Time.time >= nextRepathTime)
        {
            motor.MoveTo(
                sensor.Target.position,
                data.attackRange * 0.8f);

            nextRepathTime =
                Time.time + data.repathInterval;
        }
    }

    private void UpdateAttack()
    {
        if (!sensor.IsInsideEncounter(
                guardPoint,
                data.encounterRadius))
        {
            SetState(EnemyState.Disengage);
            return;
        }

        if (!sensor.IsInsideAttackRange(data.attackRange))
        {
            SetState(EnemyState.Chase);
            return;
        }

        motor.Face(sensor.Target.position);

        attack.TryStartAttack(sensor.Target, npcType);
    }

    private void UpdateDisengage()
    {
        if (motor.HasReachedDestination())
        {
            SetState(EnemyState.Guard);
        }
    }

    private void UpdateHurt()
    {
        if (Time.time < hurtEndTime)
        {
            return;
        }

        if (!sensor.HasActiveTarget ||
            !sensor.IsInsideEncounter(
                guardPoint,
                data.encounterRadius))
        {
            SetState(EnemyState.Disengage);
            return;
        }

        if (sensor.IsInsideAttackRange(data.attackRange))
        {
            SetState(EnemyState.Attack);
        }
        else
        {
            SetState(EnemyState.Chase);
        }
    }

    private void HandleDamaged(
        int currentHealth,
        int maximumHealth)
    {
        if (currentState == EnemyState.Defeated)
        {
            return;
        }

        SetState(EnemyState.Hurt);
    }

    private void HandleDefeated()
    {
        SetState(EnemyState.Defeated);
    }

    private void SetState(
        EnemyState newState,
        bool force = false)
    {
        if (!force && currentState == newState)
        {
            return;
        }

        EnemyState oldState = currentState;

        if (oldState == EnemyState.Attack &&
            newState != EnemyState.Attack)
        {
            attack.CancelAttack();
        }

        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Guard:
                motor.Stop();
                break;

            case EnemyState.Chase:
                nextRepathTime = 0f;
                break;

            case EnemyState.Attack:
                motor.Stop();
                break;

            case EnemyState.Disengage:
                motor.MoveTo(guardPoint, 0.05f);
                break;

            case EnemyState.Hurt:
                motor.Stop();
                AudioManager.Instance?.NPCActionResolver(npcType, CharAction.Hurt, transform.position);
                hurtEndTime =
                    Time.time + data.hitStunDuration;
                break;

            case EnemyState.Defeated:
                motor.DisableMovement();
                sensor.enabled = false;
                attack.enabled = false;
                AudioManager.Instance?.NPCActionResolver(npcType, CharAction.Death, transform.position);


                Collider enemyCollider = GetComponent<Collider>();
                if (enemyCollider != null)
                {
                    enemyCollider.enabled = false;
                }

                break;
        }

        UpdateCombatEngagement();

        if (logStateChanges)
        {
            Debug.Log(
                $"{name}: {currentState}",
                this);
        }
    }

    private void UpdateCombatEngagement()
    {
        SetCombatEngagement(IsCombatState(currentState));
    }

    private void SetCombatEngagement(bool isEngaged)
    {
        if (reportedCombatEngagement == isEngaged)
        {
            return;
        }

        reportedCombatEngagement = isEngaged;
        CombatEngagementChanged?.Invoke(this, isEngaged);
    }

    private static bool IsCombatState(EnemyState state)
    {
        return state == EnemyState.Chase ||
               state == EnemyState.Attack ||
               state == EnemyState.Hurt;
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null)
        {
            return;
        }

        Vector3 centre = Application.isPlaying
            ? guardPoint
            : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            data.detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            data.attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            centre,
            data.encounterRadius);
    }
}