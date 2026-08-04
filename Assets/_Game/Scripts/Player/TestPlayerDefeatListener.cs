using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public sealed class TestPlayerDefeatListener : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private int receivedDefeatEvents;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        playerHealth.Defeated += HandleDefeated;
    }

    private void OnDisable()
    {
        playerHealth.Defeated -= HandleDefeated;
    }

    private void HandleDefeated()
    {
        receivedDefeatEvents++;

        Debug.Log(
            $"Player defeat event received. " +
            $"Count: {receivedDefeatEvents}",
            this);
    }
}