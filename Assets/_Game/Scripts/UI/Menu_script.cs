using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
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
    private ButtonView exitButtonView;

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

    private List<Slider> VolumeSliders;

    [SerializeField]
    private MainMenuMusicManager MainMenuMusicManager;

    private void Start()
    {
        playButtonView.enabled = true;
        optionButtonView.enabled = true;
        exitButtonView.enabled = true;

        Return = InputSystem.actions.FindAction("Return");

        MasterVolume = GameObject.Find("Master Slider").GetComponent<Slider>();
        SFXVolume = GameObject.Find("SFX Slider").GetComponent<Slider>();
        UIVolume = GameObject.Find("UI Slider").GetComponent<Slider>();
        MusicVolume = GameObject.Find("Music Slider").GetComponent<Slider>();

        VolumeSliders = new List<Slider>
        {
            MasterVolume,
            SFXVolume,
            UIVolume,
            MusicVolume
        };

        // Slider's Listeners 
        VolumeSliders.ForEach(
            slider =>
            {
                slider.onValueChanged.AddListener(value => OnSliderChanged(slider, value));

                Debug.Log(slider.name);
            }
        );

        MainMenuMusicManager = GameObject.Find("PF_MainMenuMusicManager").GetComponent<MainMenuMusicManager>();
        

        MainMenuScreen.SetActive(true);
        OptionsScreen.SetActive(false);

    }
    private void OnEnable()
    {
        playButtonView.ButtonClicked += OnPlayButtonClicked;
        optionButtonView.ButtonClicked += OnOptionButtonClicked;
        exitButtonView.ButtonClicked += OnExitButtonClicked;
    }

    private void OnDisable()
    {
        playButtonView.ButtonClicked -= OnPlayButtonClicked; //Dehooks
        optionButtonView.ButtonClicked -= OnOptionButtonClicked;
        exitButtonView.ButtonClicked -= OnExitButtonClicked;
    }
    private void OnDestroy()
    {
        VolumeSliders.ForEach(
            s => s.onValueChanged.RemoveAllListeners()
        );
    }

    private void Update()
    {
        if (Return.IsPressed() == true)
        {
            OnReturn();
        }
    }

    // Handle Volume Change
    private void OnSliderChanged(Slider slider, float value)
    {
        MainMenuMusicManager.OnSliderChanged(slider, value);
    }

    private void OnPlayButtonClicked()
    {
        Debug.Log("Play Button Pressed!");

        SceneManager.LoadScene(StartScene);
    }

    private void OnOptionButtonClicked()
    {
        Debug.Log("Option Button Pressed!");
        OptionsScreen.SetActive(true);
        MainMenuScreen.SetActive(false);
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("Exit Button Pressed!");
        Application.Quit();
    }

    private void OnReturn()
    {
        if (OptionsScreen.activeSelf == true)
        {
            OptionsScreen.SetActive(false);
            MainMenuScreen.SetActive(true);
        }
    }
}
