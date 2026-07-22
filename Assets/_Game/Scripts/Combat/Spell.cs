using UnityEngine;

// Defines how a spell is cast: instantly on the move, or stationary with a load time
public enum SpellType
{
    Instant,      // cast while moving, no load time
    Stationary    // must stand still, has load time
}

// ScriptableObject holding the data for a single spell; instances are created via the Assets > Create menu
[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/SpellData")]
public class SpellData : ScriptableObject
{
    public string spellName;                      // display/identifier name of the spell
    public SpellType spellType;                   // determines casting behavior (instant vs stationary)
    public GameObject projectilePrefab;           // the visual/physical prefab spawned when cast
    public float projectileSpeed = 15f;           // launch speed of the projectile in units/sec
    public float damage = 20f;                    // damage dealt by the spell on impact
    public float cooldown = 1f;                   // time (seconds) before the spell can be cast again
    public float loadTime = 1f;                   // charge-up duration // only used for Stationary spells
}
