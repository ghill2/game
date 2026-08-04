using System;
using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1)] private int maximumHealth = 100;
    [SerializeField] private int currentHealth;

    private bool defeatEventSent;

    public event Action<int, int> HealthChanged;
    public event Action<int> Damaged;
    public event Action Defeated;

    public int MaximumHealth => maximumHealth;
    public int CurrentHealth => currentHealth;
    public bool IsAlive => currentHealth > 0;

    private void Awake()
    {
        ResetHealth();
    }

    public void TakeDamage(int amount)
    {
        if (!IsAlive || amount <= 0)
        {
            return;
        }

        int previousHealth = currentHealth;

        currentHealth = Mathf.Max(
            0,
            currentHealth - amount);

        int appliedDamage =
            previousHealth - currentHealth;

        Damaged?.Invoke(appliedDamage);
        HealthChanged?.Invoke(
            currentHealth,
            maximumHealth);

        if (currentHealth == 0 && !defeatEventSent)
        {
            defeatEventSent = true;
            Defeated?.Invoke();
        }
    }

    public void RestoreHealth(int amount)
    {
        if (!IsAlive || amount <= 0)
        {
            return;
        }

        int newHealth = Mathf.Min(
            maximumHealth,
            currentHealth + amount);

        if (newHealth == currentHealth)
        {
            return;
        }

        currentHealth = newHealth;

        HealthChanged?.Invoke(
            currentHealth,
            maximumHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maximumHealth;
        defeatEventSent = false;

        HealthChanged?.Invoke(
            currentHealth,
            maximumHealth);
    }
}