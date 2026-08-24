using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private Transform castPoint;  // the point (e.g. hand) where spells spawn from
    [SerializeField] private Crosshair crosshair;  // provides the crosshair aim point used to aim spells
    [SerializeField] private float recastDelay = 0.5f; // time (seconds) before any spell can be cast again

    private Animator _animator;                               // animator that triggers the cast animation
    private static readonly int CastHash = Animator.StringToHash("Cast"); // cached int hash for the Cast trigger (faster than string lookups)
    private Dictionary<string, GameObject> _spellMap;         // maps spell name -> prefab, loaded from Resources
    private float _nextCastTime;                              // earliest time casting is allowed again

    // fired once when a cast succeeds, carries the spell's prefab name and the cooldown duration in seconds
    public event Action<string, float> OnSpellCast;

    // Awake runs once; grab the animator and preload all spell prefabs
    void Awake()
    {
        _animator = GetComponentInChildren<Animator>(); // animator lives on a child model
        LoadSpells();                                   // build the name->prefab lookup table
    }

    // Load every spell prefab stored under Resources/Spells into a dictionary keyed by name
    void LoadSpells()
    {
        GameObject[] spells = Resources.LoadAll<GameObject>("Spells"); // load all prefabs in that folder

        _spellMap = new Dictionary<string, GameObject>();

        foreach (GameObject spell in spells)        // index each prefab by its asset name
        {
            _spellMap[spell.name] = spell;
            Debug.Log("Mapped spell: " + spell.name); // log for debugging/verification
        }
    }

    // Input bindings: each key casts a specific spell by name
    void OnF(InputValue value) => CastSpell("Human_Spell_Fireball_Large");      // F = fireball
    void OnR(InputValue value) => CastSpell("Human_Spell_Shockwave_Ground");    // R = ground shockwave
    void OnE(InputValue value) => CastSpell("Human_Spell_Shockwave_Explosion"); // E = explosion shockwave

    // Attempt to cast the named spell; aborts with an error if it isn't mapped
    void CastSpell(string spellName)
    {
        if (!_spellMap.TryGetValue(spellName, out GameObject prefab))
        {
            Debug.LogError($"Spell not found: {spellName}");
            return;
        }

        if (Time.time < _nextCastTime) // retimer, block all casts during cooldown
            return;

        if (prefab.GetComponent<FireballProjectile>() != null)
        {
            GetComponent<PlayerAudioEvents>()?.PlayFireballCast();
        }

        _animator.SetTrigger(CastHash);
        _nextCastTime = Time.time + recastDelay; // start the retimer
        OnSpellCast?.Invoke(spellName, recastDelay); // notify UI with the spell name and countdown duration
        SpawnSpell(prefab);
    }

    // Instantiate the spell prefab aimed at the crosshair and play its particles
    void SpawnSpell(GameObject prefab)
    {
        GameObject spell = Instantiate(prefab, castPoint.position, GetSpellRotation(prefab));
        IgnoreCasterCollisions(spell);

        foreach (ParticleSystem ps in spell.GetComponentsInChildren<ParticleSystem>())
            ps.Play();
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
