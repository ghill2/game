using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RetryMenu_Script : MonoBehaviour
{
    [SerializeField]
    private ButtonView retryButton;
    [SerializeField]
    private ButtonView exitButton;

    [SerializeField]
    private string mainMenuSceneName = "SCN_FINAL_MainMenu";

    private PlayerInput playerInput;
    

    private void Awake()
    {
        retryButton = GameObject.Find("Retry Button").GetComponent<ButtonView>();
        exitButton = GameObject.Find("Exit Button").GetComponent<ButtonView>();

        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();

        // Disable input towards the wizard
        playerInput.SwitchCurrentActionMap("UI/Menu");
                
    }

    private void OnEnable()
    {
        retryButton.ButtonClicked += OnRetryButtonClicked;
        exitButton.ButtonClicked += OnExitButtonClicked;
    }

    private void OnDisable()
    {
        //IMPORTANT: Always removes all listener when disabled.
        retryButton.ButtonClicked -= OnRetryButtonClicked;
        exitButton.ButtonClicked -= OnExitButtonClicked;
    }

    private void OnRetryButtonClicked()
    {
        Debug.Log("Retry Button Clicked");
        GameStateManager.Instance?.ResetCurrentLevel();
        playerInput.SwitchCurrentActionMap("Player");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //SceneManager.LoadScene("SCN_UI", LoadSceneMode.Additive);
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("Exit Button Clicked");
        // Loads Main Menu Scene
        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }
}
