using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource inGameSource;
    [SerializeField] private AudioSource inBattleSource;

    [SerializeField, Range(0f, 1f)]
    private float masterVolume = 1f;

    [SerializeField, Min(0f)]
    private float fadeDuration = 1f;

    private readonly HashSet<EnemyBrain> engagedEnemies =
        new HashSet<EnemyBrain>();

    private float battleBlend;
    private bool isBattleMusicActive;

    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = Mathf.Clamp01(value);
            ApplyVolumes();
        }
    }

    public bool IsBattleMusicActive => isBattleMusicActive;
    public int EngagedEnemyCount => engagedEnemies.Count;

    private void Awake()
    {
        if (!IsConfigured())
        {
            enabled = false;
            return;
        }

        ConfigureSource(inGameSource);
        ConfigureSource(inBattleSource);
    }

    private void OnEnable()
    {
        EnemyBrain.CombatEngagementChanged +=
            HandleCombatEngagementChanged;

        RefreshEngagedEnemies();

        isBattleMusicActive = engagedEnemies.Count > 0;
        battleBlend = isBattleMusicActive ? 1f : 0f;

        ApplyVolumes();
        StartInGameTrack();

        if (isBattleMusicActive)
        {
            StartBattleTrack();
        }
    }

    private void OnDisable()
    {
        EnemyBrain.CombatEngagementChanged -=
            HandleCombatEngagementChanged;

        engagedEnemies.Clear();

        if (inGameSource != null)
        {
            inGameSource.Stop();
        }

        if (inBattleSource != null)
        {
            inBattleSource.Stop();
        }
    }

    private void Update()
    {
        RemoveMissingEnemies();

        float targetBlend =
            isBattleMusicActive ? 1f : 0f;

        if (fadeDuration <= 0f)
        {
            battleBlend = targetBlend;
        }
        else
        {
            battleBlend = Mathf.MoveTowards(
                battleBlend,
                targetBlend,
                Time.unscaledDeltaTime / fadeDuration);
        }

        ApplyVolumes();

        if (!isBattleMusicActive &&
            battleBlend <= 0f &&
            inBattleSource.isPlaying)
        {
            inBattleSource.Stop();
        }
    }

    private void HandleCombatEngagementChanged(
        EnemyBrain enemy,
        bool isEngaged)
    {
        if (enemy == null)
        {
            return;
        }

        if (isEngaged)
        {
            engagedEnemies.Add(enemy);
        }
        else
        {
            engagedEnemies.Remove(enemy);
        }

        SetBattleMusicActive(engagedEnemies.Count > 0);
    }

    private void RefreshEngagedEnemies()
    {
        engagedEnemies.Clear();

        EnemyBrain[] enemies =
            FindObjectsByType<EnemyBrain>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);

        for (int index = 0; index < enemies.Length; index++)
        {
            if (enemies[index].IsEngagedInCombat)
            {
                engagedEnemies.Add(enemies[index]);
            }
        }
    }

    private void RemoveMissingEnemies()
    {
        int removedCount = engagedEnemies.RemoveWhere(
            enemy => enemy == null ||
                     !enemy.IsEngagedInCombat);

        if (removedCount > 0)
        {
            SetBattleMusicActive(engagedEnemies.Count > 0);
        }
    }

    private void SetBattleMusicActive(bool isActive)
    {
        if (isBattleMusicActive == isActive)
        {
            return;
        }

        isBattleMusicActive = isActive;

        if (isBattleMusicActive)
        {
            StartBattleTrack();
        }
    }

    private void StartInGameTrack()
    {
        if (!inGameSource.isPlaying)
        {
            inGameSource.Play();
        }
    }

    private void StartBattleTrack()
    {
        if (inBattleSource.isPlaying)
        {
            return;
        }

        inBattleSource.time = 0f;
        inBattleSource.Play();
    }

    private void ApplyVolumes()
    {
        if (inGameSource == null ||
            inBattleSource == null)
        {
            return;
        }

        float safeMasterVolume =
            Mathf.Clamp01(masterVolume);

        inGameSource.volume =
            safeMasterVolume * (1f - battleBlend);

        inBattleSource.volume =
            safeMasterVolume * battleBlend;
    }

    private bool IsConfigured()
    {
        if (inGameSource != null &&
            inBattleSource != null &&
            inGameSource.clip != null &&
            inBattleSource.clip != null)
        {
            return true;
        }

        Debug.LogError(
            $"{name} needs two AudioSources with music clips.",
            this);

        return false;
    }

    private static void ConfigureSource(AudioSource source)
    {
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    private void OnValidate()
    {
        masterVolume = Mathf.Clamp01(masterVolume);
        fadeDuration = Mathf.Max(0f, fadeDuration);
    }
}
