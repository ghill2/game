using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu_Script : MonoBehaviour
{
    [SerializeField] private ButtonView resumeButtonView;
    [SerializeField] private ButtonView optionButtonView;
    [SerializeField] private ButtonView exitButtonView;
    [SerializeField] private ButtonView returnButtonView;

    [Header("Volume Sliders")]
    [SerializeField] private Slider MasterVolume;
    [SerializeField] private Slider SFXVolume;
    [SerializeField] private Slider UIVolume;
    [SerializeField] private Slider MusicVolume;

    [SerializeField] private GameObject MainMenuScreen;
    [SerializeField] private GameObject OptionsScreen;

    [SerializeField] private string mainMenuSceneName = "SCN_MainMenu";

    private InputAction Return;

    public event Action Unpause;
    private void Awake()
    {
        Debug.Log("PauseMenu_Script Awake()");
        resumeButtonView.enabled = true;
        optionButtonView.enabled = true;
        exitButtonView.enabled = true;

        Return = InputSystem.actions?.FindAction("Return");

        MainMenuScreen.SetActive(true);
        OptionsScreen.SetActive(false);
    }

    private void Start()
    {
        SyncVolumeSliders();
    }

    private void OnEnable()
    {
        Debug.Log("PauseMenu_Script OnEnable()");
        //Listeners -- LHS : <Listening> += <Action Response> : RHS
        resumeButtonView.ButtonClicked += OnResumeButtonClicked;
        optionButtonView.ButtonClicked += OnOptionButtonClicked;
        exitButtonView.ButtonClicked += OnExitButtonClicked;
        if (returnButtonView != null) returnButtonView.ButtonClicked += OnReturn;
        BindVolumeSliders(true);
        GameStateManager.AudioSettingsChanged += SyncVolumeSliders;
        SyncVolumeSliders();
    }

    private void OnDisable()
    {
        //IMPORTANT: Always removes all listener when disabled.
        resumeButtonView.ButtonClicked -= OnResumeButtonClicked;
        optionButtonView.ButtonClicked -= OnOptionButtonClicked;
        exitButtonView.ButtonClicked -= OnExitButtonClicked;
        if (returnButtonView != null) returnButtonView.ButtonClicked -= OnReturn;
        BindVolumeSliders(false);
        GameStateManager.AudioSettingsChanged -= SyncVolumeSliders;
    }

    private void Update()
    {
        if (Return?.WasPressedThisFrame() == true)
        {
            OnReturn();
        }
    }

    private void OnResumeButtonClicked()
    {
        Debug.Log("Resume Button Pressed!");
        AudioManager.Instance?.PlayUIConfirmSFX();
        Resume();        
    }

    private void OnOptionButtonClicked()
    {
        Debug.Log("Option Button Pressed!");
        AudioManager.Instance?.PlayUISelectSFX();
        SyncVolumeSliders();
        OptionsScreen.SetActive(true);
        MainMenuScreen.SetActive(false);
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("RtMN Button Pressed!");
        AudioManager.Instance?.PlayUICancelSFX();
        //Save progress, logic tbd
        //stub
        Save();

        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }

    private void Resume()
    {
        Unpause?.Invoke();
    }

    private void OnReturn()
    {
        if (OptionsScreen.activeSelf == true)
        {
            OptionsScreen.SetActive(false);
            MainMenuScreen.SetActive(true);
        }
        else if (MainMenuScreen.activeSelf == true)
        {
            Resume();    
        }
    }


    private void BindVolumeSliders(bool bind)
    {
        if (MasterVolume != null)
        {
            if (bind) MasterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
            else MasterVolume.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        }
        if (SFXVolume != null)
        {
            if (bind) SFXVolume.onValueChanged.AddListener(OnSFXVolumeChanged);
            else SFXVolume.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }
        if (UIVolume != null)
        {
            if (bind) UIVolume.onValueChanged.AddListener(OnUIVolumeChanged);
            else UIVolume.onValueChanged.RemoveListener(OnUIVolumeChanged);
        }
        if (MusicVolume != null)
        {
            if (bind) MusicVolume.onValueChanged.AddListener(OnMusicVolumeChanged);
            else MusicVolume.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        }
    }

    private void SyncVolumeSliders()
    {
        GameStateManager state = GameStateManager.Instance;
        if (state == null) return;

        if (MasterVolume != null) MasterVolume.SetValueWithoutNotify(state.MasterVolume);
        if (SFXVolume != null) SFXVolume.SetValueWithoutNotify(state.SFXVolume);
        if (UIVolume != null) UIVolume.SetValueWithoutNotify(state.UIVolume);
        if (MusicVolume != null) MusicVolume.SetValueWithoutNotify(state.MusicVolume);
    }

    private void OnMasterVolumeChanged(float value)
    {
        GameStateManager.Instance?.SetMasterVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        GameStateManager.Instance?.SetSFXVolume(value);
    }

    private void OnUIVolumeChanged(float value)
    {
        GameStateManager.Instance?.SetUIVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        GameStateManager.Instance?.SetMusicVolume(value);
    }

    //Saves the current game state (?)
    private void Save()
    {
        //Stub
    }
}
