using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

    [SerializeField] private AudioClip fireballCastClip;
    [SerializeField] private AudioClip[] ouchClips;
    [SerializeField] private AudioClip collectedClip;
    [SerializeField] private AudioClip step1Clip;
    [SerializeField] private AudioClip step2Clip;

    [SerializeField] private AudioClip skeletonDefeatedClip;
    [SerializeField] private AudioClip wolfDefeatedClip;

    [SerializeField] private AudioClip fireballHitClip;
    [SerializeField, Min(0.01f)] private float spatialMinDistance = 1f;
    [SerializeField, Min(0.01f)] private float spatialMaxDistance = 35f;

    [SerializeField] private AudioClip portalClip;
    [SerializeField, Min(0f)] private float portalFadeDuration = 0.3f;

    private AudioSource twoDimensionalSource;
    private bool portalTransitionStarted;

    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = Mathf.Clamp01(value);
            ApplyMasterVolume();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another AudioManager is already active. This instance is disabled.", this);
            enabled = false;
            return;
        }

        Instance = this;
        twoDimensionalSource = GetComponent<AudioSource>();
        twoDimensionalSource.playOnAwake = false;
        twoDimensionalSource.loop = false;
        twoDimensionalSource.spatialBlend = 0f;
        ApplyMasterVolume();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnValidate()
    {
        masterVolume = Mathf.Clamp01(masterVolume);
        spatialMinDistance = Mathf.Max(0.01f, spatialMinDistance);
        spatialMaxDistance = Mathf.Max(spatialMinDistance, spatialMaxDistance);
        portalFadeDuration = Mathf.Max(0f, portalFadeDuration);

        if (Application.isPlaying && Instance == this)
        {
            ApplyMasterVolume();
        }
    }

    public void PlayFireballCast()
    {
        PlayTwoDimensional(fireballCastClip);
    }

    public void PlayFireballHit(Vector3 position)
    {
        PlaySpatial(fireballHitClip, position);
    }

    public void PlayRandomOuch()
    {
        if (ouchClips == null || ouchClips.Length == 0)
        {
            Debug.LogWarning("AudioManager has no ouch clips assigned.", this);
            return;
        }

        PlayTwoDimensional(ouchClips[Random.Range(0, ouchClips.Length)]);
    }

    public void PlayCollected()
    {
        PlayTwoDimensional(collectedClip);
    }

    public void PlayStep1()
    {
        PlayTwoDimensional(step1Clip);
    }

    public void PlayStep2()
    {
        PlayTwoDimensional(step2Clip);
    }

    public bool PlayPortalTransition(string targetSceneName)
    {
        if (portalTransitionStarted || string.IsNullOrWhiteSpace(targetSceneName))
        {
            return false;
        }

        portalTransitionStarted = true;
        StartCoroutine(PortalTransitionRoutine(targetSceneName));
        return true;
    }

    private IEnumerator PortalTransitionRoutine(string targetSceneName)
    {
        CanvasGroup fadeOverlay = CreateFadeOverlay();
        PlayTwoDimensional(portalClip);

        float soundDuration = portalClip != null ? portalClip.length : 0f;
        float transitionDuration = Mathf.Max(portalFadeDuration, soundDuration);
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeOverlay.alpha = portalFadeDuration <= 0f
                ? 1f
                : Mathf.Clamp01(elapsed / portalFadeDuration);
            yield return null;
        }

        fadeOverlay.alpha = 1f;
        SceneManager.LoadScene(targetSceneName);
    }

    private void PlayTwoDimensional(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager tried to play an unassigned clip.", this);
            return;
        }

        twoDimensionalSource.PlayOneShot(clip);
    }

    private void PlaySpatial(AudioClip clip, Vector3 position)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager tried to play an unassigned spatial clip.", this);
            return;
        }

        GameObject soundObject = new GameObject("One Shot - " + clip.name);
        soundObject.transform.position = position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = spatialMinDistance;
        source.maxDistance = spatialMaxDistance;
        source.Play();

        Destroy(soundObject, clip.length + 0.1f);
    }

    private CanvasGroup CreateFadeOverlay()
    {
        GameObject canvasObject = new GameObject(
            "Portal Fade",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasGroup),
            typeof(GraphicRaycaster));

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        CanvasGroup canvasGroup = canvasObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        GameObject imageObject = new GameObject(
            "Black",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));

        imageObject.transform.SetParent(canvasObject.transform, false);

        RectTransform imageTransform = imageObject.GetComponent<RectTransform>();
        imageTransform.anchorMin = Vector2.zero;
        imageTransform.anchorMax = Vector2.one;
        imageTransform.offsetMin = Vector2.zero;
        imageTransform.offsetMax = Vector2.zero;

        Image image = imageObject.GetComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = true;

        return canvasGroup;
    }

    private void ApplyMasterVolume()
    {
        AudioListener.volume = masterVolume;
    }

    public void PlaySkeletonDefeated(Vector3 position)
    {
        PlaySpatial(skeletonDefeatedClip, position);
    }

    public void PlayWolfDefeated(Vector3 position)
    {
        PlaySpatial(wolfDefeatedClip, position);
    }
}
