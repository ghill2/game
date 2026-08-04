using UnityEngine;

public sealed class MockPlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField, Min(1)] private int maximumHealth = 100;

    public int CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0;

    private void Awake()
    {
        CurrentHealth = maximumHealth;
    }

    public void TakeDamage(int amount)
    {
        if (!IsAlive || amount <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

        Debug.Log(
            $"Mock player health: {CurrentHealth}/{maximumHealth}",
            this);

        if (!IsAlive)
        {
            Debug.Log("Mock player defeated.", this);
        }
    }
}
