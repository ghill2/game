using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton_View : MonoBehaviour
{
    [SerializeField] private Button button;

    public event Action PlayButtonClicked;

    
    private void OnEnable()
    {
        button.onClick.AddListener(onPlayButtonClicked);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(onPlayButtonClicked);
    }
    private void onPlayButtonClicked()
    {
        PlayButtonClicked?.Invoke();
    }
}
