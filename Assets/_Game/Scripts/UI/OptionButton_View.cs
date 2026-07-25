using UnityEngine;
using System;
using UnityEngine.UI;

public class OptionButton_View : MonoBehaviour
{
    [SerializeField] private Button button;

    public event Action OptionButtonClicked;

    private void OnEnable()
    {
        button.onClick.AddListener(onOptionButtonClicked);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(onOptionButtonClicked);
    }
    private void onOptionButtonClicked()
    {
        OptionButtonClicked?.Invoke();
    }
}
