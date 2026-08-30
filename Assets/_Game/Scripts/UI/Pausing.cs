using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Pausing : MonoBehaviour
{
    private GameObject player;
    private InputAction pause;
    [SerializeField] private PlayerInput playerInput;
    private PauseMenu_Script PauseMenu;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInput = player.GetComponent<PlayerInput>();
        //Variable Initialize
        pause = InputSystem.actions.FindAction("Pause");
        //Sets Player Input Scheme
        playerInput.SwitchCurrentActionMap("Player");
        Debug.Log(playerInput.currentActionMap);

        //Sets Game tick
        Time.timeScale = 1f;
    }

    private void OnDisable()
    {
        //IMPORTANT: Always removes all listener when disabled.
        if (PauseMenu != null) PauseMenu.Unpause -= OnUnpause;
    }

    private void Update()
    {
        if (pause?.WasPressedThisFrame() == true)
        {
            OnPauseButtonPressed();
        }
    }

    private void OnPauseButtonPressed()
    {
        Debug.Log("Player Pause is pressed!");
        StartCoroutine(OpenPauseMenu());
    }
    private IEnumerator OpenPauseMenu()
    {
        Time.timeScale = 0f; //Freezes game tick
        playerInput.SwitchCurrentActionMap("UI/Menu");
        
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("SCN_PauseMenu", LoadSceneMode.Additive);
        yield return loadOp;
        
        PauseMenu = FindFirstObjectByType<PauseMenu_Script>();

        if (PauseMenu == null)
        {
            Debug.LogError("Finding PauseMenu_Script FAILED");
            yield break;
        }

        //Adding Listeners -- LHS : <Listening> += <Action Response> : RHS
        PauseMenu.Unpause += OnUnpause;
    }

    private void OnUnpause()
    {
        playerInput.SwitchCurrentActionMap("Player");
        Time.timeScale = 1f; //Resumes game tick

        //Removing Listeners
        PauseMenu.Unpause -= OnUnpause;

        //Unloads Pause Menu
        SceneManager.UnloadSceneAsync("SCN_PauseMenu");       
    }
}
