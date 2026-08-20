using System.Collections.Generic;
using UnityEngine;

public class BossPortalController : MonoBehaviour
{
    private List<EnemySpawner> spawners;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to all children EnemySpawner instances' AllEnemiesSpawned event
        spawners = new List<EnemySpawner>(GetComponentsInChildren<EnemySpawner>());

        foreach (var spawner in spawners)
        {
            Debug.Log($"Subscribing to AllEnemiesSpawned event of spawner: {spawner.name}");
            spawner.AllEnemiesSpawned += HandleAllEnemiesSpawned;
        }
    }

    private void HandleAllEnemiesSpawned(EnemySpawner spawner)
    {
        // Handle the event when all enemies are spawned
        Debug.Log($"All enemies have been spawned by spawner: {spawner.name}");
        spawner.AllEnemiesSpawned -= HandleAllEnemiesSpawned;
        spawners.Remove(spawner);

        if (spawners.Count == 0)
        {
            Debug.Log("All EnemySpawner instances have completed spawning.");
            Destroy(gameObject);
        }
    }
}
