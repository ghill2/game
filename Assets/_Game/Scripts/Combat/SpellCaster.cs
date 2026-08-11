using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private Transform castPoint;  // the point (e.g. hand) where spells spawn from
    [SerializeField] private Crosshair crosshair;  // provides the crosshair aim point used to aim spells

    private Animator _animator;                               // animator that triggers the cast animation
    private static readonly int CastHash = Animator.StringToHash("Cast"); // cached int hash for the Cast trigger (faster than string lookups)
    private Dictionary<string, GameObject> _spellMap;         // maps spell name -> prefab, loaded from Resources

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
        if (!_spellMap.TryGetValue(spellName, out GameObject prefab)) // look up the prefab by name
        {
            Debug.LogError($"Spell not found: {spellName}"); // missing entry means it wasn't loaded
            return;
        }

        _animator.SetTrigger(CastHash); // trigger the cast animation
        SpawnSpell(prefab);             // create and play the spell effect
    }

    // Instantiate the spell prefab aimed at the crosshair and play any particle effects it contains
    void SpawnSpell(GameObject prefab)
    {
        GameObject spell = Instantiate(prefab, castPoint.position, GetSpellRotation());

        // Find and start every particle system nested under the spell prefab
        foreach (ParticleSystem ps in spell.GetComponentsInChildren<ParticleSystem>())
            ps.Play();
    }

    // Rotation that aims the spell from the cast point at the crosshair target (computed here)
    Quaternion GetSpellRotation()
    {
        if (crosshair == null || castPoint == null) return castPoint != null ? castPoint.rotation : transform.rotation; // graceful fallback
        Vector3 dir = crosshair.GetAimPoint() - castPoint.position; // hand -> crosshair target
        if (dir.sqrMagnitude <= 0.0001f) return castPoint.rotation;               // avoid zero-direction LookRotation
        return Quaternion.LookRotation(dir.normalized);
    }
}
