using System;
using UnityEngine;

public sealed class EnemyHealth : MonoBehaviour, IDamageable
{
    private enum DefeatSound
    {
        None = 0,
        Skeleton = 1,
        Wolf = 2
    }

    private static readonly int DeathTrigger = Animator.StringToHash("Death");

    [SerializeField]
    private int maximumHealth;
    [SerializeField]
    private int currentHealth;

    [SerializeField]
    private DefeatSound defeatSound;

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

            PlayDefeatSound();
            Defeated?.Invoke();
        }
    }

    private void PlayDefeatSound()
    {
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            return;
        }

        switch (defeatSound)
        {
            case DefeatSound.Skeleton:
                audioManager.PlaySkeletonDefeated(transform.position);
                break;

            case DefeatSound.Wolf:
                audioManager.PlayWolfDefeated(transform.position);
                break;
        }
    }
}
