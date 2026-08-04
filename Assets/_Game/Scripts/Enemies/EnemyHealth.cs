using System;
using UnityEngine;

public sealed class EnemyHealth : MonoBehaviour, IDamageable
{
    private static readonly int DeathTrigger =  Animator.StringToHash("Death");

    [SerializeField]
    private int maximumHealth;
    [SerializeField]
    private int currentHealth;

    [SerializeField]
    private Animator animator;

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

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
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
            if (animator != null)
            {
                animator.SetTrigger(DeathTrigger);
            }
            Defeated?.Invoke();
        }
    }
}