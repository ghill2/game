using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Menu_script : MonoBehaviour
{
    [SerializeField] private ButtonView playButtonView;
    [SerializeField] private ButtonView optionButtonView;
    [SerializeField] private ButtonView exitButtonView;

    [SerializeField] private GameObject MainMenuScreen;
    [SerializeField] private GameObject OptionsScreen;

    private InputAction Return;

    private const string CharacterScene = "SCN_Main8";
    //Change this if the level to load is different
    private const string LevelToLoad = "SCN_Level_0_Tutorial-Level";
    private void Start()
    {
        playButtonView.enabled = true;
        optionButtonView.enabled = true;
        exitButtonView.enabled = true;

        Return = InputSystem.actions.FindAction("Return");

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

    private void Update()
    {
        if (Return.IsPressed() == true)
        {
            OnReturn();
        }
    }

    private void OnPlayButtonClicked()
    {
        Debug.Log("Play Button Pressed!");

        SceneManager.LoadScene(CharacterScene);
        SceneManager.LoadScene(LevelToLoad, LoadSceneMode.Additive);
        SceneManager.LoadScene("SCN_UI", LoadSceneMode.Additive);
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
