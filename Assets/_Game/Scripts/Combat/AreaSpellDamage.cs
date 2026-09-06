using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Applies one damage pulse after the spell effect starts.
public sealed class AreaSpellDamage : MonoBehaviour
{
    public enum AOEAreaShape
    {
        Sphere,
        Box
    }

    [SerializeField] private AOEAreaShape shape;
    [SerializeField, Min(1)] private int damage = 20;
    [SerializeField, Min(0f)] private float hitDelay = 0.25f;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Vector3 localCenter = Vector3.up;
    [SerializeField, Min(0.1f)] private float radius = 3f;
    [SerializeField] private Vector3 boxSize = new Vector3(2f, 2f, 5f);

    private Transform caster;
    private bool initialized;
    private bool didDamage;
    private readonly HashSet<IDamageable> damagedTargets = new();

    public void Initialize(Transform spellCaster)
    {
        if (initialized) return;

        caster = spellCaster;
        initialized = true;
    }

    private IEnumerator Start()
    {
        // SpellCaster sets the owner before Start runs.
        if (!initialized || caster == null) yield break;

        if (hitDelay > 0f)
            yield return new WaitForSeconds(hitDelay);

        ApplyDamage();
    }

    private void ApplyDamage()
    {
        if (didDamage || caster == null) return;
        didDamage = true;

        // Determine the center and real scale of the AOE attack.
        Vector3 center = transform.TransformPoint(localCenter);
        Vector3 scale = transform.lossyScale;
        scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));

        // A single query per cast has no fixed buffer limit for enemy groups.
        Collider[] hits = shape == AOEAreaShape.Box
            ? Physics.OverlapBox(center, Vector3.Scale(boxSize, scale) * 0.5f,
                transform.rotation, enemyLayers, QueryTriggerInteraction.Collide)
            : Physics.OverlapSphere(center, radius * Mathf.Max(scale.x, scale.y, scale.z),
                enemyLayers, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            Component targetComponent = target as Component;

            if (targetComponent == null || !target.IsAlive ||
                targetComponent.transform.IsChildOf(caster) ||
                hit.transform.IsChildOf(caster))
                continue;

            // Several colliders can belong to the same enemy.
            if (damagedTargets.Add(target))
                target.TakeDamage(damage);
        }
    }

    // Draw the AOE shapes for debugging.
    private void OnDrawGizmosSelected()
    {
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;
        Gizmos.color = Color.cyan;

        if (shape == AOEAreaShape.Box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(localCenter, boxSize);
        }
        else
        {
            Vector3 scale = transform.lossyScale;
            float worldRadius = radius * Mathf.Max(
                Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            Gizmos.DrawWireSphere(transform.TransformPoint(localCenter), worldRadius);
        }

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }
}
