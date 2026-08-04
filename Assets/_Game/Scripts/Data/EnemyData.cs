using UnityEngine;

[CreateAssetMenu(
    fileName = "DA_Enemy",
    menuName = "Game/Enemies/Enemy Data")]
public sealed class EnemyData : ScriptableObject
{
    [Header("Health")]
    [Min(1)] public int maximumHealth = 30;
    [Min(0f)] public float hitStunDuration = 0.2f;

    [Header("Movement")]
    [Min(0.1f)] public float moveSpeed = 3.5f;
    [Min(1f)] public float angularSpeed = 720f;

    [Header("Detection")]
    [Min(0.1f)] public float detectionRange = 8f;
    [Min(0.1f)] public float encounterRadius = 14f;

    [Header("Attack")]
    [Min(0.1f)] public float attackRange = 1.8f;
    [Min(0.1f)] public float attackHitRadius = 1f;
    [Min(1)] public int attackDamage = 10;

    [Tooltip("Time before the damage window opens.")]
    [Min(0f)] public float attackWindupDuration = 0.3f;

    [Tooltip("Time when the attack can cause damage.")]
    [Min(0.01f)] public float attackWindowDuration = 0.2f;

    [Min(0.1f)] public float attackCooldown = 1.2f;

    [Header("Navigation")]
    [Min(0.05f)] public float repathInterval = 0.15f;

    private void OnValidate()
    {
        maximumHealth = Mathf.Max(1, maximumHealth);
        hitStunDuration = Mathf.Max(0f, hitStunDuration);

        attackRange = Mathf.Max(0.1f, attackRange);
        attackHitRadius = Mathf.Max(0.1f, attackHitRadius);
        attackWindupDuration = Mathf.Max(0f, attackWindupDuration);
        attackWindowDuration = Mathf.Max(0.01f, attackWindowDuration);

        float minimumCooldown =
            attackWindupDuration + attackWindowDuration;

        attackCooldown = Mathf.Max(
            attackCooldown,
            minimumCooldown);

        detectionRange = Mathf.Max(attackRange, detectionRange);
        encounterRadius = Mathf.Max(detectionRange, encounterRadius);
    }
}