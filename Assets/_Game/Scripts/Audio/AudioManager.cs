//using GameAudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Collections.Specialized.BitVector32;
using static Unity.VisualScripting.Member;
[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixer mixer;

    [System.Serializable]
    public class CategoryBinding
    {
        public SoundCategory category;
        public AudioMixerGroup mixerGroup;
        //public string exposedVolumeParam;
    }

    [Header("Category -> Mixer Group bindings")]
    public List<CategoryBinding> categoryBindings = new List<CategoryBinding>();


    [Header("Mixer Snapshots - Environment")]
    public AudioMixerSnapshot snap_shot_stage_00;
    public AudioMixerSnapshot snap_shot_stage_01;
    public AudioMixerSnapshot snap_shot_stage_02;
    [SerializeField] private float snap_shot_transition_time_environment = 2f;


    [Header("Mixer Snapshots - Music")]
    public AudioMixerSnapshot snap_shot_music_cautious;
    public AudioMixerSnapshot snap_shot_music_battle;
    [SerializeField] private float snap_shot_transition_time_music = 2f;


    [Header("Environment Loop")]
    [SerializeField] private List<SoundEvent> evt_env_stages;


    [Header("Player SFX")]
    [SerializeField] private SoundEvent evt_player_footstep_generic;
    [SerializeField] private SoundEvent evt_player_vox_attack;
    [SerializeField] private SoundEvent evt_player_vox_death;
    [SerializeField] private SoundEvent evt_player_vox_hurt;
    [SerializeField] private SoundEvent evt_player_jump;

    [Header("Spells SFX")]
    [SerializeField] private SoundEvent evt_spell_fireball_attack;
    [SerializeField] private SoundEvent evt_spell_fireball_impact;
    [SerializeField] private SoundEvent evt_spell_ice_attack;
    [SerializeField] private SoundEvent evt_spell_ice_impact;
    [SerializeField] private SoundEvent evt_spell_lightning_attack;
    [SerializeField] private SoundEvent evt_spell_lightning_impact;


    [System.Serializable]
    public class NPCActionBinding
    {
        public NpcType npc;
        public SoundEvent evt_npc_attack;
        public SoundEvent evt_npc_death;
        public SoundEvent evt_npc_melee_attack;
    }

    [Header("NPC Binding")]
    public List<NPCActionBinding> npcActionBinding = new List<NPCActionBinding>();

    [Header("Props")]
    [SerializeField] private SoundEvent evt_props_portal;
    [SerializeField] private SoundEvent evt_props_portal_enter;
    [SerializeField] private SoundEvent evt_props_windmill;
    [SerializeField] private SoundEvent evt_props_magic_orb;


    [Header("UI SFX")]
    [SerializeField] private SoundEvent evt_ui_cancel;
    [SerializeField] private SoundEvent evt_ui_confirm;
    [SerializeField] private SoundEvent evt_ui_pickup_egg;
    [SerializeField] private SoundEvent evt_ui_pickup_boot;
    [SerializeField] private SoundEvent evt_ui_pickup_potion;
    [SerializeField] private SoundEvent evt_ui_pickup_generic;
    [SerializeField] private SoundEvent evt_ui_select;
    [SerializeField] private SoundEvent evt_ui_speedup_effect;


    [Header("UI Mixer Faders")]
    [SerializeField] private String mixer_master_volume_param;
    [SerializeField] private String mixer_music_volume_param;
    [SerializeField] private String mixer_sfx_volume_param;
    [SerializeField] private String mixer_ui_volume_param;




    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

 
    [SerializeField] private AudioClip skeletonDefeatedClip;
    [SerializeField] private AudioClip wolfDefeatedClip;

    [SerializeField] private AudioClip fireballHitClip;
    [SerializeField, Min(0.01f)] private float spatialMinDistance = 1f;
    [SerializeField, Min(0.01f)] private float spatialMaxDistance = 35f;

    [SerializeField] private AudioClip portalClip;
    [SerializeField, Min(0f)] private float portalFadeDuration = 0.3f;

    private AudioSource twoDimensionalSource;
    private List<AudioSource> environmentSources;
    private bool portalTransitionStarted;    
    public AudioSource playerSource { get; set; }
    [HideInInspector] public int current_level = 0;
    private GameObject player;
    private SpellCaster spellCaster;
    private PlayerCollectibles playerCollectibles;

    private bool hasStarted;


    //---------------------------------Adam's New Audio Code----------------------------
    public float GetDecibleVal(float _sliderVal)
    {
        float clampedValue = Mathf.Clamp(_sliderVal, 0.0001f, 1f);

        float decibelValue = Mathf.Log10(clampedValue) * 20;

        return decibelValue;
    }
    public void SetMasterVolume(float _new_vol)
    {
        mixer.SetFloat(mixer_master_volume_param, GetDecibleVal(_new_vol));
    }

    public void SetMusicVolume(float _new_vol)
    {
        mixer.SetFloat(mixer_music_volume_param, GetDecibleVal(_new_vol));
    }

    public void SetSFXVolume(float _new_vol)
    {
        mixer.SetFloat(mixer_sfx_volume_param, GetDecibleVal(_new_vol));
    }
    public void SetUXVolume(float _new_vol)
    {
        mixer.SetFloat(mixer_ui_volume_param, GetDecibleVal(_new_vol));
    }

    public void PropsSFXResolver(PropsType _propsType, AudioSource _src)
    {
        if (_src)
        {
            switch (_propsType)
            {
                case PropsType.orb:
                    PlaySoundEvent(evt_props_magic_orb, _src);
                    break;
                case PropsType.windmill:
                    PlaySoundEvent(evt_props_windmill, _src);
                    break;
                case PropsType.smoke:
                    break;
                case PropsType.portal:
                    PlaySoundEvent(evt_props_portal, _src);
                    break;
                default: break;
            }
        }
    }

    public void PlayerActionResolver(CharAction _action)
    {      
        switch (_action)
        {           
            case CharAction.Hurt:
                PlaySoundEvent(evt_player_vox_hurt, playerSource);
                break;
            case CharAction.Jump:
                PlaySoundEvent(evt_player_jump, playerSource);
                break;
            case CharAction.Death:
                PlaySoundEvent(evt_player_vox_death, playerSource);
                break;
            case CharAction.Step:
                PlaySoundEvent(evt_player_footstep_generic, playerSource);
                break;
            case CharAction.CollectEgg:
                PlaySoundEvent(evt_ui_pickup_egg, playerSource);
                break;
            case CharAction.CollectBoot:
                PlaySoundEvent(evt_ui_pickup_boot, playerSource);
                PlaySoundEvent(evt_ui_speedup_effect);

                break;
            case CharAction.CollectPotion:
                PlaySoundEvent(evt_ui_pickup_potion, playerSource);
                break;
            case CharAction.CollectGeneric:
                PlaySoundEvent(evt_ui_pickup_generic, playerSource);
                break;
            default: break;
        }
    }

    
    public void SpellCastResolver(SpellId _spellID, float recastDelay)
    {
        switch (_spellID)
        {
            case SpellId.Fireball:
                PlaySoundEvent(evt_spell_fireball_attack,playerSource);
                PlaySoundEvent(evt_player_vox_attack, playerSource);
                break;
            case SpellId.Frostblast:
                PlaySoundEvent(evt_spell_ice_attack, playerSource);
                PlaySoundEvent(evt_player_vox_attack, playerSource);
                break;
            case SpellId.ElectricStorm:
                PlaySoundEvent(evt_spell_lightning_attack, playerSource);
                PlaySoundEvent(evt_player_vox_attack, playerSource);
                break;
            default: break;
        }        
    }


    public void NPCActionResolver(NpcType _npcType, CharAction _action, Vector3 _pos)
    {
        
        foreach (var n in npcActionBinding)
        {
            if (n.npc == _npcType)
            {
                switch (_action)
                {
                    case CharAction.Attack:
                        PlaySoundEvent(n.evt_npc_attack, _pos);
                        PlaySoundEvent(n.evt_npc_melee_attack, _pos);
                        break;
                    case CharAction.Death:
                        SoundEvent deathEvent = n.evt_npc_death;
                        //Always play sound when npc dies
                        deathEvent.probability = 1;
                        PlaySoundEvent(deathEvent, _pos);                        
                        break;
                    default: break;
                }
            }
        }
    }

    private AudioMixerGroup GetGroup(SoundCategory category)
    {
        foreach (var b in categoryBindings)
            if (b.category == category) return b.mixerGroup;
        return null;
    }
    private void InitEnvironmentSource()
    {
        environmentSources = new List<AudioSource>();

        foreach (var b in evt_env_stages)
        {
            environmentSources.Add(gameObject.AddComponent<AudioSource>());
        }
    }


    public void PlayEnvironmentStageSwitch(int switch_to_stage = 0)
    {
        switch (switch_to_stage)
        {
            case 0:
                PlaySoundEvent(evt_env_stages[0], environmentSources[0]);
                snap_shot_stage_00.TransitionTo(snap_shot_transition_time_environment);
                StartCoroutine(StopAfterDelay(environmentSources[1]));
                StartCoroutine(StopAfterDelay(environmentSources[2]));
                break;
            case 1:
                PlaySoundEvent(evt_env_stages[1], environmentSources[1]);
                snap_shot_stage_01.TransitionTo(snap_shot_transition_time_environment);
                StartCoroutine(StopAfterDelay(environmentSources[0]));
                break;
            case 2:
                PlaySoundEvent(evt_env_stages[2], environmentSources[2]);
                snap_shot_stage_02.TransitionTo(snap_shot_transition_time_environment);
                StartCoroutine(StopAfterDelay(environmentSources[1]));
                break;
        }

    }

    IEnumerator StopAfterDelay(AudioSource src, float delay = 2f)
    {
        yield return new WaitForSeconds(delay);
        src.Stop();
    }

    public AudioSource PlaySoundEvent(SoundEvent _evt)
    {

        return PlaySoundEvent(_evt, Vector3.zero);
    }

    public AudioSource PlaySoundEvent (SoundEvent _evt, AudioSource _src)
    {
        return PlaySoundEvent(_evt, Vector3.zero, _src);
    }


    public AudioSource PlaySoundEvent(SoundEvent _evt, Vector3 _position, AudioSource _src = null)
    {
        AudioSource source;
        GameObject soundObject = null;
        AudioClip clip = _evt.GetClip();
        if (clip == null)
        {
            Debug.LogWarning("Clip is empty in sound event!!");
            return null;
        }

        if (_src != null)
        {
            Debug.Log("***********have source");
            source = _src;
        }
        else if (_evt.spatialBlend == 0 && !_evt.loop)
        {
            Debug.Log("***********2d sound");
            source = twoDimensionalSource;
        }
        else
        {
            Debug.Log("**********3d");
            soundObject = new GameObject("One Shot - " + clip.name);
            soundObject.transform.position = _position;
            source = soundObject.AddComponent<AudioSource>();
            Destroy(soundObject, clip.length + 0.1f);
        }

        //configure source
        source.volume = _evt.GetVolume();
        source.pitch = _evt.GetPitch();
        source.loop = _evt.loop;
        source.dopplerLevel = 0f;
        source.spatialBlend = _evt.spatialBlend;
        source.minDistance = _evt.minDistance;
        source.maxDistance = _evt.maxDistance;
        source.rolloffMode = _evt.rolloffMode;
        source.outputAudioMixerGroup = _evt.mixerGroup ? _evt.mixerGroup : GetGroup(_evt.category);
        source.clip = clip;
        source.time = _evt.startRandomTime ? UnityEngine.Random.Range(0f, clip.length) : 0f;

        if (UnityEngine.Random.Range(0f, 1f) <= _evt.probability)
        {
            //Debug.Log("***************Playing " + source.clip.name);

            if (_evt.loop) source.Play();
            else source.PlayOneShot(clip);
        } 

        if (_evt.spatialBlend == 0 && !_evt.loop) { Destroy(soundObject, source.clip.length + 0.1f); }
        return _evt.loop ? source : null;
    }

    //fading music from battle to cautious
    public void mixer_fade_to_cautious_music()
    {
        snap_shot_music_cautious.TransitionTo(snap_shot_transition_time_music);
    }

    public void mixer_fade_to_battle_music()
    {
        snap_shot_music_battle.TransitionTo(snap_shot_transition_time_music);

    }




    //-------------------------------------------------------------------------------------


    //public float MasterVolume
    //{
    //    get => masterVolume;
    //    set
    //    {
    //        masterVolume = Mathf.Clamp01(value);
    //        ApplyMasterVolume();
    //    }
    //}

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
        InitEnvironmentSource();
    }

    private void OnEnable()
    {
        GameStateManager.AudioSettingsChanged += ApplyGameStateVolumes;
        if (hasStarted) ApplyGameStateVolumes();
    }

    private void OnDisable()
    {
        GameStateManager.AudioSettingsChanged -= ApplyGameStateVolumes;
    }

    private void ApplyGameStateVolumes()
    {
        GameStateManager state = GameStateManager.Instance;
        if (!hasStarted || Instance != this || state == null || mixer == null)
        {
            return;
        }

        SetMasterVolume(state.MasterVolume);
        SetMusicVolume(state.MusicVolume);
        SetSFXVolume(state.SFXVolume);
        SetUXVolume(state.UIVolume);
    }

    private void Start()
    {
        // Apply mixer settings in Start, after all audio objects have loaded.
        hasStarted = true;
        ApplyGameStateVolumes();

        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        spellCaster = player.GetComponent<SpellCaster>();

        if (spellCaster != null)
        {
            spellCaster.OnSpellCast += SpellCastResolver;
        }
        else
        {
            Debug.LogError("SpellCaster component not found on the player.");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (spellCaster != null)
        {
            spellCaster.OnSpellCast -= SpellCastResolver;
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

    public void PlayFireballHit(Vector3 position)
    {
        PlaySoundEvent(evt_spell_fireball_impact, position);
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
        PlaySoundEvent(evt_props_portal_enter, transform.position);

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
            typeof(UnityEngine.UI.Image)
            );

        imageObject.transform.SetParent(canvasObject.transform, false);

        RectTransform imageTransform = imageObject.GetComponent<RectTransform>();
        imageTransform.anchorMin = Vector2.zero;
        imageTransform.anchorMax = Vector2.one;
        imageTransform.offsetMin = Vector2.zero;
        imageTransform.offsetMax = Vector2.zero;

        UnityEngine.UI.Image image = imageObject.GetComponent<UnityEngine.UI.Image>();
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
        //PlaySpatial(skeletonDefeatedClip, position);
    }

    public void PlayWolfDefeated(Vector3 position)
    {
        PlaySpatial(wolfDefeatedClip, position);
    }
}

