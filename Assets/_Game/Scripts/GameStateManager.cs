using System;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
public sealed class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public static event Action AudioSettingsChanged;
    public event Action ProgressChanged;

    public int TotalScore { get; private set; }
    public int CurrentLevelScore { get; private set; }
    public bool IsGameRunning { get; private set; }

    private double gameStartTime;
    private double finishedGameTime;
    private bool currentLevelCompleted;

    // Real time includes pauses, retry screens, and scene loading.
    public double TotalGameTimeSeconds => IsGameRunning
        ? Math.Max(0d, Time.realtimeSinceStartupAsDouble - gameStartTime)
        : finishedGameTime;

    [Header("Audio Settings")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float uiVolume = 1f;

    public float MasterVolume
    {
        get => masterVolume;
        set => SetVolume(ref masterVolume, value);
    }

    public float MusicVolume
    {
        get => musicVolume;
        set => SetVolume(ref musicVolume, value);
    }

    public float SFXVolume
    {
        get => sfxVolume;
        set => SetVolume(ref sfxVolume, value);
    }

    public float UIVolume
    {
        get => uiVolume;
        set => SetVolume(ref uiVolume, value);
    }

    public float EffectiveMusicVolume => masterVolume * musicVolume;
    public float EffectiveSFXVolume => masterVolume * sfxVolume;
    public float EffectiveUIVolume => masterVolume * uiVolume;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        Instance = null;
        AudioSettingsChanged = null;
    }

    private void Awake()
    {
        // Check for duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Ensure we're in the root
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        DontDestroyOnLoad(gameObject);
        AudioSettingsChanged?.Invoke();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Public methods to set volumes without events
    public void SetMasterVolume(float value) => MasterVolume = value;
    public void SetMusicVolume(float value) => MusicVolume = value;
    public void SetSFXVolume(float value) => SFXVolume = value;
    public void SetUIVolume(float value) => UIVolume = value;

    public void StartNewGame()
    {
        TotalScore = 0;
        CurrentLevelScore = 0;
        currentLevelCompleted = false;
        finishedGameTime = 0d;
        gameStartTime = Time.realtimeSinceStartupAsDouble;
        IsGameRunning = true;
        ProgressChanged?.Invoke();
    }

    // Called when the player enters a level or retries it.
    public void ResetCurrentLevel()
    {
        CurrentLevelScore = 0;
        currentLevelCompleted = false;
        ProgressChanged?.Invoke();
    }

    public void AddEggScore(int eggCount = 1)
    {
        if (!IsGameRunning || currentLevelCompleted || eggCount <= 0)
        {
            return;
        }

        CurrentLevelScore += eggCount;
        ProgressChanged?.Invoke();
    }

    // Only an exit portal can add the level score to the total.
    public void CompleteCurrentLevel()
    {
        if (!IsGameRunning || currentLevelCompleted)
        {
            return;
        }

        TotalScore += CurrentLevelScore;
        CurrentLevelScore = 0;
        currentLevelCompleted = true;
        ProgressChanged?.Invoke();
    }

    public void FinishGame()
    {
        if (!IsGameRunning)
        {
            return;
        }

        finishedGameTime = TotalGameTimeSeconds;
        IsGameRunning = false;
        ProgressChanged?.Invoke();
    }

    public void ResetProgress()
    {
        TotalScore = 0;
        CurrentLevelScore = 0;
        currentLevelCompleted = false;
        gameStartTime = 0d;
        finishedGameTime = 0d;
        IsGameRunning = false;
        ProgressChanged?.Invoke();
    }

    private static float LimitVolume(float value)
    {
        return float.IsNaN(value) ? 0f : Mathf.Clamp01(value);
    }

    private void SetVolume(ref float volume, float value)
    {
        value = LimitVolume(value);
        if (volume == value)
        {
            return;
        }

        volume = value;
        if (Instance == this)
        {
            AudioSettingsChanged?.Invoke();
        }
    }

    // Editor validation (just in case)
    private void OnValidate()
    {
        masterVolume = LimitVolume(masterVolume);
        musicVolume = LimitVolume(musicVolume);
        sfxVolume = LimitVolume(sfxVolume);
        uiVolume = LimitVolume(uiVolume);
    }
}
