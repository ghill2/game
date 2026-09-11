using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_script : MonoBehaviour
{
    [SerializeField]
    private ButtonView playButtonView;

    [SerializeField]
    private ButtonView optionButtonView;

    [SerializeField]
    private ButtonView returnButtonView;

    [SerializeField]
    private GameObject MainMenuScreen;
    
    [SerializeField]
    private GameObject OptionsScreen;

    [SerializeField]
    private string StartScene = "SCN_Level_0_Tutorial-Cave";

    private InputAction Return;

    [SerializeField]
    private Slider MasterVolume;
    [SerializeField]
    private Slider SFXVolume;
    [SerializeField]
    private Slider UIVolume;
    [SerializeField]
    private Slider MusicVolume;

    [SerializeField]
    private MainMenuMusicManager MainMenuMusicManager;

    private void Start()
    {
        playButtonView.enabled = true;
        optionButtonView.enabled = true;

        Return = InputSystem.actions?.FindAction("Return");

        SyncVolumeSliders();

        MainMenuScreen.SetActive(true);
        OptionsScreen.SetActive(false);

    }
    private void OnEnable()
    {
        playButtonView.ButtonClicked += OnPlayButtonClicked;
        optionButtonView.ButtonClicked += OnOptionButtonClicked;
        if (returnButtonView != null) returnButtonView.ButtonClicked += OnReturn;
        BindVolumeSliders(true);
        GameStateManager.AudioSettingsChanged += SyncVolumeSliders;
        SyncVolumeSliders();
    }

    private void OnDisable()
    {
        playButtonView.ButtonClicked -= OnPlayButtonClicked; //Dehooks
        optionButtonView.ButtonClicked -= OnOptionButtonClicked;
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

    private void OnPlayButtonClicked()
    {
        // Debug.Log("Play Button Pressed!");
        AudioManager.Instance?.PlayUIConfirmSFX();
        GameStateManager.Instance?.StartNewGame();
        SceneManager.LoadScene(StartScene);
        //SceneManager.LoadScene("SCN_UI", LoadSceneMode.Additive);
    }

    private void OnOptionButtonClicked()
    {
        // Debug.Log("Option Button Pressed!");
        AudioManager.Instance?.PlayUISelectSFX();
        SyncVolumeSliders();
        OptionsScreen.SetActive(true);
        MainMenuScreen.SetActive(false);
    }

    private void OnReturn()
    {
        if (OptionsScreen.activeSelf == true)
        {
            AudioManager.Instance?.PlayUICancelSFX();
            OptionsScreen.SetActive(false);
            MainMenuScreen.SetActive(true);
        }
    }
}
