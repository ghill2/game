using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_script : MonoBehaviour
{
    [SerializeField] private ButtonView playButtonView;
    [SerializeField] private ButtonView optionButtonView;
    [SerializeField] private ButtonView exitButtonView;
    [SerializeField] private GameObject MainMenuScreen;
    [SerializeField] private GameObject OptionsScreen;
    [SerializeField] private string StartScene = "SCN_Level_0_Tutorial-Cave";

    [SerializeField] private Slider MasterVolume;
    [SerializeField] private Slider SFXVolume;
    [SerializeField] private Slider UIVolume;
    [SerializeField] private Slider MusicVolume;

    [SerializeField]
    private ButtonView returnButtonView;

    private InputAction Return;

    private void Awake()
    {
        // Older menu prefabs may not have slider references yet.
        Slider[] sliders = GetComponentsInChildren<Slider>(true);
        MasterVolume = FindSlider(MasterVolume, sliders, "Master Slider");
        SFXVolume = FindSlider(SFXVolume, sliders, "SFX Slider");
        UIVolume = FindSlider(UIVolume, sliders, "UI Slider");
        MusicVolume = FindSlider(MusicVolume, sliders, "Music Slider");
    }

    private void Start()
    {
        if (playButtonView != null) playButtonView.enabled = true;
        if (optionButtonView != null) optionButtonView.enabled = true;
        if (exitButtonView != null)
        {
            exitButtonView.enabled = false;
            exitButtonView.gameObject.SetActive(false);
        }

        Return = InputSystem.actions?.FindAction("Return");
        if (MainMenuScreen != null) MainMenuScreen.SetActive(true);
        if (OptionsScreen != null) OptionsScreen.SetActive(false);
        RefreshVolumeSliders();
    }

    private void OnEnable()
    {
        if (playButtonView != null) playButtonView.ButtonClicked += OnPlayButtonClicked;
        if (optionButtonView != null) optionButtonView.ButtonClicked += OnOptionButtonClicked;
        if (exitButtonView != null) exitButtonView.ButtonClicked += OnExitButtonClicked;

        if (returnButtonView != null) returnButtonView.ButtonClicked += OnReturn;

        if (MasterVolume != null) MasterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (SFXVolume != null) SFXVolume.onValueChanged.AddListener(OnSFXVolumeChanged);
        if (UIVolume != null) UIVolume.onValueChanged.AddListener(OnUIVolumeChanged);
        if (MusicVolume != null) MusicVolume.onValueChanged.AddListener(OnMusicVolumeChanged);

        GameStateManager.AudioSettingsChanged += RefreshVolumeSliders;
        RefreshVolumeSliders();
    }

    private void OnDisable()
    {
        if (playButtonView != null) playButtonView.ButtonClicked -= OnPlayButtonClicked;
        if (optionButtonView != null) optionButtonView.ButtonClicked -= OnOptionButtonClicked;
        if (exitButtonView != null) exitButtonView.ButtonClicked -= OnExitButtonClicked;

        if (MasterVolume != null) MasterVolume.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        if (SFXVolume != null) SFXVolume.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        if (UIVolume != null) UIVolume.onValueChanged.RemoveListener(OnUIVolumeChanged);
        if (MusicVolume != null) MusicVolume.onValueChanged.RemoveListener(OnMusicVolumeChanged);

        GameStateManager.AudioSettingsChanged -= RefreshVolumeSliders;
    }

    private static Slider FindSlider(Slider assigned, Slider[] sliders, string sliderName)
    {
        if (assigned != null) return assigned;

        foreach (Slider slider in sliders)
        {
            if (slider.name == sliderName) return slider;
        }

        return null;
    }

    private void RefreshVolumeSliders()
    {
        GameStateManager state = GameStateManager.Instance;
        if (state == null) return;

        // Update the controls without sending their change events back.
        if (MasterVolume != null) MasterVolume.SetValueWithoutNotify(state.MasterVolume);
        if (SFXVolume != null) SFXVolume.SetValueWithoutNotify(state.SFXVolume);
        if (UIVolume != null) UIVolume.SetValueWithoutNotify(state.UIVolume);
        if (MusicVolume != null) MusicVolume.SetValueWithoutNotify(state.MusicVolume);
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (GameStateManager.Instance != null) GameStateManager.Instance.MasterVolume = value;
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (GameStateManager.Instance != null) GameStateManager.Instance.SFXVolume = value;
    }

    private void OnUIVolumeChanged(float value)
    {
        if (GameStateManager.Instance != null) GameStateManager.Instance.UIVolume = value;
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (GameStateManager.Instance != null) GameStateManager.Instance.MusicVolume = value;
    }

    private void Update()
    {
        if (Return != null && Return.WasPressedThisFrame())
        {
            OnReturn();
        }
    }

    private void OnPlayButtonClicked()
    {
        Debug.Log("Play Button Pressed!");
        SceneManager.LoadScene(StartScene);
        // SceneManager.LoadScene("SCN_UI", LoadSceneMode.Additive);
    }

    private void OnOptionButtonClicked()
    {
        if (OptionsScreen != null) OptionsScreen.SetActive(true);
        if (MainMenuScreen != null) MainMenuScreen.SetActive(false);
        RefreshVolumeSliders();
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();
    }

    private void OnReturn()
    {
        if (OptionsScreen != null && OptionsScreen.activeSelf)
        {
            OptionsScreen.SetActive(false);
            if (MainMenuScreen != null) MainMenuScreen.SetActive(true);
        }
    }
}
