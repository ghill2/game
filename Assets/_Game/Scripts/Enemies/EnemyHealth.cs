using System;
using UnityEngine;

public sealed class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maximumHealth;
    [SerializeField] private int currentHealth;

    private bool defeatEventSent;

    public event Action<int, int> Damaged;
    public event Action Defeated;

    public int MaximumHealth => maximumHealth;
    public int CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0;

    public void Configure(int newMaximumHealth)
    {
        maximumHealth = Mathf.Max(1, newMaximumHealth);
        currentHealth = maximumHealth;
        defeatEventSent = false;
    }

    public void TakeDamage(int amount)
    {
        if (!IsAlive || amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);

        Damaged?.Invoke(currentHealth, maximumHealth);

        if (currentHealth == 0 && !defeatEventSent)
        {
            defeatEventSent = true;
            Defeated?.Invoke();
        }
    }
}