using UnityEngine;
using UnityEngine.InputSystem;

public class TestDamageSource : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 10;
    [SerializeField, Min(1f)] private float range = 50f;

    [SerializeField]
    private GameObject EnemyObject;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null &&
            keyboard.digit1Key.wasPressedThisFrame)
        {
            FireTestHit();
        }
    }

    private void FireTestHit()
    {
        IDamageable damageable = EnemyObject.GetComponentInChildren<IDamageable>();

        if (damageable == null)
        {
            Debug.LogWarning(
                "No IDamageable found on the target object.",
                this);
            return;
        }

        damageable?.TakeDamage(damage);
    }
}
