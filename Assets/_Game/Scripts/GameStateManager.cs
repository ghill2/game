using System;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
public sealed class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public static event Action AudioSettingsChanged;
    public event Action ProgressChanged;

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

    public void ResetProgress()
    {
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
