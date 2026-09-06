using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

// unique identifier for each castable spell
public enum SpellId
{
    Fireball,
    Frostblast,
    ElectricStorm
}

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private Transform castPoint;  // the point (e.g. hand) where spells spawn from
    [SerializeField] private Crosshair crosshair;  // provides the crosshair aim point used to aim spells
    [SerializeField] private float recastDelay = 0.5f; // time (seconds) before any spell can be cast again

    private Animator _animator;                               // animator that triggers the cast animation
    private static readonly int CastHash = Animator.StringToHash("Cast"); // cached int hash for the Cast trigger (faster than string lookups)

    // maps each SpellId to the name of its prefab under Resources/Spells
    private static readonly Dictionary<SpellId, string> SpellPrefabNames = new()
    {
        { SpellId.Fireball, "Human_Spell_Fireball_Large" },
        { SpellId.Frostblast, "Human_Spell_Ice" },
        { SpellId.ElectricStorm, "Human_Spell_LightningStrike" }
    };

    private Dictionary<SpellId, GameObject> _spellMap;        // maps SpellId -> prefab, loaded from Resources
    private float _nextCastTime;                              // earliest time casting is allowed again
    private SpellId _lastSpell = SpellId.Fireball;            // most recently cast spell; recast by left mouse button

    // fired once when a cast succeeds, carries the spell cast and the cooldown duration in seconds
    public event Action<SpellId, float> OnSpellCast;

    // Awake runs once; grab the animator and preload all spell prefabs
    void Awake()
    {
        _animator = GetComponentInChildren<Animator>(); // animator lives on a child model
        LoadSpells();                                   // build the SpellId->prefab lookup table
    }

    // Load each mapped spell prefab from Resources/Spells into a dictionary keyed by SpellId
    void LoadSpells()
    {
        _spellMap = new Dictionary<SpellId, GameObject>();

        foreach (KeyValuePair<SpellId, string> entry in SpellPrefabNames)
        {
            GameObject prefab = Resources.Load<GameObject>($"Spells/{entry.Value}");
            if (prefab == null)                       // abort mapping this spell if the prefab is missing
            {
                Debug.LogError($"Spell prefab not found: {entry.Value}");
                continue;
            }

            _spellMap[entry.Key] = prefab;
        }
    }

    // Input bindings: each key casts a specific spell
    void OnF(InputValue value) => CastSpell(SpellId.Fireball);      // F = fireball
    void OnR(InputValue value) => CastSpell(SpellId.Frostblast);    // R = frost blast
    void OnE(InputValue value) => CastSpell(SpellId.ElectricStorm); // E = electric storm
    void OnLastSpell(InputValue value) => CastSpell(_lastSpell);    // left mouse = recast the last spell

    // Attempt to cast the spell; aborts with an error if it isn't mapped
    void CastSpell(SpellId spellId)
    {
        if (!_spellMap.TryGetValue(spellId, out GameObject prefab))
        {
            Debug.LogError($"Spell not found: {spellId}");
            return;
        }

        if (Time.time < _nextCastTime) // retimer, block all casts during cooldown
            return;

        if (!TryGetSpellTargetDirection(spellId, prefab, out Vector3 position, out Quaternion rotation))
            return;

        if (prefab.GetComponent<FireballProjectile>() != null)
        {
            GetComponent<PlayerAudioEvents>()?.PlayFireballCast();
        }

        _animator.SetTrigger(CastHash);
        _nextCastTime = Time.time + recastDelay; // start the retimer
        _lastSpell = spellId;                    // remember successful casts for the left-mouse recast
        OnSpellCast?.Invoke(spellId, recastDelay); // notify UI with the spell and countdown duration
        SpawnSpell(prefab, position, rotation);
    }

    // Set the damage owner before the new spell starts.
    void SpawnSpell(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject spell = Instantiate(prefab, position, rotation);
        spell.GetComponent<AreaSpellDamage>()?.Initialize(transform);
        IgnoreCasterCollisions(spell);

        foreach (ParticleSystem ps in spell.GetComponentsInChildren<ParticleSystem>())
            ps.Play();
    }

    bool TryGetSpellTargetDirection(SpellId spellId, GameObject prefab,
        out Vector3 position, out Quaternion rotation)
    {
        position = transform.position;
        rotation = Quaternion.identity;
        if (castPoint == null) return false;

        if (spellId == SpellId.Fireball)
        {
            position = castPoint.position;
            rotation = GetSpellRotation(prefab);
            return true;
        }

        // The camera can arrive later when scenes load additively.
        if (crosshair == null)
            crosshair = FindFirstObjectByType<Crosshair>();

        if (spellId == SpellId.Frostblast)
        {
            if (!TryGetGroundPointForAOEAttack(transform.position, out position))
                return false;

            Vector3 forward = crosshair != null
                ? crosshair.transform.forward
                : transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude <= 0.0001f)
                forward = transform.forward;

            rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
            return true;
        }

        if (crosshair == null) return false;

        // Ignore the player, projectiles, and trigger volumes when choosing a strike.
        int aimLayers = LayerMask.GetMask("Default", "Ground", "Level", "Enemy");
        if (!Physics.Raycast(crosshair.transform.position, crosshair.transform.forward,
                out RaycastHit hit, 100f, aimLayers, QueryTriggerInteraction.Ignore))
            return false;

        if (hit.collider.GetComponentInParent<IDamageable>() != null)
            return TryGetGroundPointForAOEAttack(hit.point, out position);

        // Skip walls and ceilings. A miss does not use the cooldown.
        if (Vector3.Dot(hit.normal, Vector3.up) < 0.5f)
            return false;

        position = hit.point;
        return true;
    }

    bool TryGetGroundPointForAOEAttack(Vector3 point, out Vector3 groundPoint)
    {
        int groundLayers = LayerMask.GetMask("Default", "Ground", "Level");
        if (Physics.Raycast(point + Vector3.up * 2f, Vector3.down,
                out RaycastHit hit, 6f, groundLayers, QueryTriggerInteraction.Ignore) &&
            Vector3.Dot(hit.normal, Vector3.up) >= 0.5f)
        {
            groundPoint = hit.point;
            return true;
        }

        groundPoint = point;
        return false;
    }

    // Aim the projectile so its collider touches the surface under the crosshair
    Quaternion GetSpellRotation(GameObject prefab)
    {
        if (crosshair == null || castPoint == null)
            return castPoint != null ? castPoint.rotation : transform.rotation;

        Vector3 dir = GetProjectileAimPoint(prefab) - castPoint.position;
        if (dir.sqrMagnitude <= 0.0001f) return castPoint.rotation;
        return Quaternion.LookRotation(dir.normalized);
    }

    Vector3 GetProjectileAimPoint(GameObject prefab)
    {
        Vector3 aimPoint = crosshair.GetAimPoint(out RaycastHit hit);
        if (hit.collider == null || prefab.GetComponent<FireballProjectile>() == null)
            return aimPoint;

        SphereCollider sphere = prefab.GetComponentInChildren<SphereCollider>();
        if (sphere == null) return aimPoint;

        Vector3 scale = sphere.transform.lossyScale;
        float worldRadius = sphere.radius * Mathf.Max(
            Mathf.Abs(scale.x / 2f),
            Mathf.Abs(scale.y / 2f),
            Mathf.Abs(scale.z / 2f));

        return aimPoint + hit.normal * worldRadius;
    }

    // A spell can start inside the caster collider, so ignore only that pair
    void IgnoreCasterCollisions(GameObject spell)
    {
        Collider[] spellColliders = spell.GetComponentsInChildren<Collider>();
        Collider[] casterColliders = GetComponentsInChildren<Collider>();

        foreach (Collider spellCollider in spellColliders)
        {
            foreach (Collider casterCollider in casterColliders)
                Physics.IgnoreCollision(spellCollider, casterCollider, true);
        }
    }
}
