using UnityEngine;
using System;
using UnityEngine.UI;

public class ExitButton_View : MonoBehaviour
{
    [SerializeField] private Button button;

    public event Action ExitButtonClicked;

    
    private void OnEnable()
    {
        button.onClick.AddListener(onExitButtonClicked);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(onExitButtonClicked);
    }
    private void onExitButtonClicked()
    {
        Debug.Log("onExitButtonClicked()");
        ExitButtonClicked?.Invoke();
    }
}
