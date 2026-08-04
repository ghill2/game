using System;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private PlayerHealth playerHealth;

    private PlayerCollectibles playerCollectibles;

    private HUDBarController healthBarController;

    private HUDCollectiblesController hudCollectiblesController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        // Get PlayerHealth component from the player
        playerHealth = player.GetComponent<PlayerHealth>();
        playerCollectibles = player.GetComponent<PlayerCollectibles>();

        playerHealth.OnHealthChanged += UpdateHealthUI;
        playerCollectibles.OnEggCollected += UpdateEggsCount;

        var hpBar = GameObject.Find("HP Bar");
        healthBarController = hpBar.GetComponentInChildren<HUDBarController>();

        hudCollectiblesController = GetComponentInChildren<HUDCollectiblesController>();
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        Debug.Log($"Updating health UI: Current Health = {currentHealth}, Max Health = {maxHealth}");
        healthBarController.UpdateBar((float)currentHealth / maxHealth);
    }

    private void UpdateEggsCount(int count)
    {
        Debug.Log($"Updating eggs count UI: Eggs Collected = {count}");
        hudCollectiblesController.SetEggsCount(count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
