using UnityEngine;
using System;

public class Menu_script : MonoBehaviour
{
    [SerializeField] private PlayButton_View playButtonView;
    [SerializeField] private OptionButton_View optionButtonView;
    [SerializeField] private ExitButton_View exitButtonView;

    private void Start()
    {
        playButtonView.enabled = true;
        optionButtonView.enabled = true;
        exitButtonView.enabled = true;
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

    private void OnPlayButtonClicked() 
    {
        Debug.Log("Play Button Pressed!");
        Debug.Log("Requires Further Logic.");
    }

    private void OnOptionButtonClicked()
    {
        Debug.Log("Option Button Pressed!");
        Debug.Log("Requires Further Logic.");
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("Exit Button Pressed!");
        Application.Quit();
    }
}
