using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu_Script : MonoBehaviour
{
    [SerializeField] private ButtonView resumeButtonView;
    [SerializeField] private ButtonView optionButtonView;
    [SerializeField] private ButtonView exitButtonView;

    [SerializeField] private GameObject MainMenuScreen;
    [SerializeField] private GameObject OptionsScreen;

    private InputAction Return;

    public event Action Unpause;
    private void Awake()
    {
        Debug.Log("PauseMenu_Script Awake()");
        resumeButtonView.enabled = true;
        optionButtonView.enabled = true;
        exitButtonView.enabled = true;

        Return = InputSystem.actions.FindAction("Return");

        MainMenuScreen.SetActive(true);
        OptionsScreen.SetActive(false);
    }

    private void OnEnable()
    {
        Debug.Log("PauseMenu_Script OnEnable()");
        //Listeners -- LHS : <Listening> += <Action Response> : RHS
        resumeButtonView.ButtonClicked += OnResumeButtonClicked;
        optionButtonView.ButtonClicked += OnOptionButtonClicked;
        exitButtonView.ButtonClicked += OnExitButtonClicked;
    }

    private void OnDisable()
    {
        //IMPORTANT: Always removes all listener when disabled.
        resumeButtonView.ButtonClicked -= OnResumeButtonClicked;
        optionButtonView.ButtonClicked -= OnOptionButtonClicked;
        exitButtonView.ButtonClicked -= OnExitButtonClicked;
    }

    private void Update()
    {
        if (Return.WasPressedThisFrame() == true)
        {
            OnReturn();
        }
    }

    private void OnResumeButtonClicked()
    {
        Debug.Log("Resume Button Pressed!");
        Resume();        
    }

    private void OnOptionButtonClicked()
    {
        Debug.Log("Option Button Pressed!");
        OptionsScreen.SetActive(true);
        MainMenuScreen.SetActive(false);
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("RtMN Button Pressed!");

        //Save progress, logic tbd
        //stub
        Save();

        SceneManager.LoadScene("SCN_MainMenu", LoadSceneMode.Single);
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

    //Saves the current game state (?)
    private void Save()
    {
        //Stub
    }
}
