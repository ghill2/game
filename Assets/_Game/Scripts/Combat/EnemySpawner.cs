using System;
using System.Collections;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string EnemyTag = "Enemy";
    private const float SpawnPointGizmoRadius = 0.25f;
    private const float OccupancyProbeRadius = 0.05f;

    [SerializeField]
    private bool isActive = true;

    [SerializeField]
    private GameObject enemyPrefab;
    
    [SerializeField, Min(0)]
    private int enemiesToSpawn = 1;

    [Tooltip("Delay between enemy spawns in milliseconds.")]
    [SerializeField, Min(0)]
    private int spawnDelayMilliseconds;

    [SerializeField, Min(0f)]
    private float activationDistance = 30f;

    [SerializeField, Min(0f)]
    private float jitterX = 2f;
    [SerializeField, Min(0f)]
    private float jitterZ = 2f;

    [SerializeField, Min(0)]
    private int spawnRetries = 3;

    [SerializeField]
    private bool enableSmoke = true;

    [SerializeField]
    private GameObject smokeEffectObject;

    private Transform player;
    private Coroutine spawnRoutine;
    private int spawnedEnemyCount;
    private bool wasPlayerInside;
    private bool allEnemiesSpawnedEventSent;

    public event Action<EnemySpawner> AllEnemiesSpawned;

    public bool IsActive
    {
        get => isActive;
        set
        {
            if (isActive == value)
            {
                return;
            }

            isActive = value;

            if (!isActive)
            {
                CancelSpawnRoutine();
                wasPlayerInside = false;
            }
        }
    }

    public GameObject EnemyPrefab => enemyPrefab;
    public int EnemiesToSpawn => enemiesToSpawn;
    public int SpawnDelayMilliseconds => spawnDelayMilliseconds;
    public float ActivationDistance => activationDistance;
    public float JitterX => jitterX;
    public float JitterZ => jitterZ;
    public int SpawnRetries => spawnRetries;
    public int SpawnedEnemyCount => spawnedEnemyCount;

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag(PlayerTag);

        player = playerObject != null
            ? playerObject.transform
            : null;

        if (!enableSmoke)
        {
            if (smokeEffectObject != null)
            {
                smokeEffectObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            CancelSpawnRoutine();
            wasPlayerInside = false;
            return;
        }

        if (player == null ||
            spawnRoutine != null ||
            spawnedEnemyCount >= enemiesToSpawn)
        {
            return;
        }

        bool isPlayerInside =
            IsPlayerWithinActivationDistance();

        if (isPlayerInside && !wasPlayerInside)
        {
            spawnRoutine =
                StartCoroutine(SpawnSequence());
        }

        wasPlayerInside = isPlayerInside;
    }

    private IEnumerator SpawnSequence()
    {
        while (spawnedEnemyCount < enemiesToSpawn)
        {
            if (!isActive)
            {
                spawnRoutine = null;
                yield break;
            }

            if (!TrySpawnEnemy())
            {
                spawnRoutine = null;
                yield break;
            }

            spawnedEnemyCount++;

            if (spawnedEnemyCount >= enemiesToSpawn)
            {
                CompleteSpawning();
                AllEnemiesSpawned?.Invoke(this);
                spawnRoutine = null;

                if (enableSmoke)
                {
                    DisableSmokeEffect();
                }

                yield break;
            }

            // The next spawn is planned while the player is still inside.
            if (!IsPlayerWithinActivationDistance())
            {
                spawnRoutine = null;
                yield break;
            }

            if (spawnDelayMilliseconds > 0)
            {
                yield return new WaitForSeconds(
                    spawnDelayMilliseconds / 1000f);
            }
        }

        spawnRoutine = null;
    }

    private bool TrySpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            return false;
        }

        int positionChecks =
            Mathf.Max(0, spawnRetries) + 1;

        for (int checkIndex = 0;
             checkIndex < positionChecks;
             checkIndex++)
        {
            Vector3 spawnPosition =
                GetRandomSpawnPosition();

            if (IsOccupiedByEnemy(spawnPosition))
            {
                continue;
            }

            GameObject spawnedEnemy = Instantiate(
                enemyPrefab,
                spawnPosition,
                enemyPrefab.transform.rotation);

            spawnedEnemy.tag = EnemyTag;
            return true;
        }

        return false;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float safeJitterX =
            Mathf.Max(0f, jitterX);

        float safeJitterZ =
            Mathf.Max(0f, jitterZ);

        return transform.position +
               new Vector3(
                   UnityEngine.Random.Range(
                       -safeJitterX,
                       safeJitterX),
                   0f,
                   UnityEngine.Random.Range(
                       -safeJitterZ,
                       safeJitterZ));
    }

    private static bool IsOccupiedByEnemy(
        Vector3 spawnPosition)
    {
        Physics.SyncTransforms();

        GameObject[] taggedEnemies =
            GameObject.FindGameObjectsWithTag(EnemyTag);

        float probeSqrRadius =
            OccupancyProbeRadius * OccupancyProbeRadius;

        for (int index = 0;
             index < taggedEnemies.Length;
             index++)
        {
            if ((taggedEnemies[index].transform.position -
                 spawnPosition).sqrMagnitude <=
                probeSqrRadius)
            {
                return true;
            }
        }

        Collider[] colliders = Physics.OverlapSphere(
            spawnPosition,
            OccupancyProbeRadius,
            Physics.AllLayers,
            QueryTriggerInteraction.Collide);

        for (int index = 0;
             index < colliders.Length;
             index++)
        {
            Transform current =
                colliders[index].transform;

            while (current != null)
            {
                if (current.gameObject.CompareTag(EnemyTag))
                {
                    return true;
                }

                current = current.parent;
            }
        }

        return false;
    }

    private bool IsPlayerWithinActivationDistance()
    {
        if (player == null)
        {
            return false;
        }

        float safeDistance =
            Mathf.Max(0f, activationDistance);

        return (player.position - transform.position).sqrMagnitude <=
               safeDistance * safeDistance;
    }

    private void CompleteSpawning()
    {
        if (allEnemiesSpawnedEventSent)
        {
            return;
        }

        allEnemiesSpawnedEventSent = true;
        AllEnemiesSpawned?.Invoke(this);
    }

    private void OnDisable()
    {
        CancelSpawnRoutine();
        wasPlayerInside = false;
    }

    private void OnValidate()
    {
        enemiesToSpawn =
            Mathf.Max(0, enemiesToSpawn);

        spawnDelayMilliseconds =
            Mathf.Max(0, spawnDelayMilliseconds);

        activationDistance =
            Mathf.Max(0f, activationDistance);

        jitterX =
            Mathf.Max(0f, jitterX);

        jitterZ =
            Mathf.Max(0f, jitterZ);

        spawnRetries =
            Mathf.Max(0, spawnRetries);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(
            transform.position,
            SpawnPointGizmoRadius);

        Matrix4x4 previousMatrix =
            Gizmos.matrix;

        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            Quaternion.identity,
            new Vector3(
                Mathf.Max(0f, jitterX),
                0.05f,
                Mathf.Max(0f, jitterZ)));

        Gizmos.DrawWireSphere(
            Vector3.zero,
            1f);

        Gizmos.matrix = previousMatrix;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(
            transform.position,
            Mathf.Max(0f, activationDistance));
    }

    private void CancelSpawnRoutine()
    {
        if (spawnRoutine == null)
        {
            return;
        }

        StopCoroutine(spawnRoutine);
        spawnRoutine = null;
    }

    private void DisableSmokeEffect()
    {
        if (enableSmoke)
        {
            enableSmoke = false;

            if (smokeEffectObject != null)
            {
                smokeEffectObject.SetActive(false);
            }
        }
    }
}
