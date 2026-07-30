using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonView : MonoBehaviour
{
    [SerializeField] private Button button;

    public event Action ButtonClicked;

    
    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }
    private void OnButtonClicked()
    {
        ButtonClicked?.Invoke();
    }
}
