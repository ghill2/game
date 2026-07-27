using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Menu_script : MonoBehaviour
{
    [SerializeField] private PlayButton_View playButtonView;
    [SerializeField] private OptionButton_View optionButtonView;
    [SerializeField] private ExitButton_View exitButtonView;

    [SerializeField] private GameObject MainMenuScreen;
    [SerializeField] private GameObject OptionsScreen;

    private InputAction Return;
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
        playButtonView.PlayButtonClicked += OnPlayButtonClicked;
        optionButtonView.OptionButtonClicked += OnOptionButtonClicked;
        exitButtonView.ExitButtonClicked += OnExitButtonClicked;
    }

    private void OnDisable()
    {
        playButtonView.PlayButtonClicked -= OnPlayButtonClicked; //Dehooks
        optionButtonView.OptionButtonClicked -= OnOptionButtonClicked;
        exitButtonView.ExitButtonClicked -= OnExitButtonClicked;
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
        Debug.Log("Requires Further Logic.");
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
