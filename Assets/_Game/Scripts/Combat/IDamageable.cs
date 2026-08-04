using UnityEngine;

public interface IDamageable
{
    bool IsAlive { get; }

    void TakeDamage(int amount);
}