using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class MainMenuMusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;

    [SerializeField, Range(0f, 1f)]
    private float masterVolume = 1f;

    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = Mathf.Clamp01(value);
            ApplyVolume();
        }
    }

    private void Awake()
    {
        if (!IsConfigured())
        {
            enabled = false;
            return;
        }

        ConfigureSource();
        ApplyVolume();
    }

    private void OnEnable()
    {
        if (musicSource != null &&
            musicSource.clip != null &&
            !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    private void OnDisable()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    private void ApplyVolume()
    {
        if (musicSource == null)
        {
            return;
        }

        musicSource.volume = Mathf.Clamp01(masterVolume);
    }

    private bool IsConfigured()
    {
        if (musicSource != null &&
            musicSource.clip != null)
        {
            return true;
        }

        Debug.LogError(
            $"{name} needs an AudioSource with a music clip.",
            this);

        return false;
    }

    private void ConfigureSource()
    {
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
    }

    private void OnValidate()
    {
        masterVolume = Mathf.Clamp01(masterVolume);
        ConfigureSourceIfAssigned();
        ApplyVolume();
    }

    private void ConfigureSourceIfAssigned()
    {
        if (musicSource != null)
        {
            ConfigureSource();
        }
    }

    public void OnSliderChanged(Slider slider, float value)
    {
        switch (slider.name)
        {
            case "Master Slider":
                Debug.Log($"Slider {slider.name} : {value}");
                AudioManager.Instance?.SetMasterVolume(value);
                break;
            case "SFX Slider":
                Debug.Log($"Slider {slider.name} : {value}");
                AudioManager.Instance.SetSFXVolume(value);
                break;
            case "UX Slider":
                Debug.Log($"Slider {slider.name} : {value}");
                AudioManager.Instance.SetUXVolume(value);
                break;
            case "Music Slider":
                Debug.Log($"Slider {slider.name} : {value}");
                AudioManager.Instance.SetMusicVolume(value);
                break;
        }
    }
}
