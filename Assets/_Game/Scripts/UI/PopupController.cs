using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [SerializeField]
    private Image Window;
    [SerializeField]
    private RawImage windowIcon;
    [SerializeField]
    private TextMeshProUGUI windowText;

    public bool showPopup = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Window = GetComponent<Image>();
        windowIcon = GetComponentInChildren<RawImage>();
        windowText = GetComponentInChildren<TextMeshProUGUI>();

        Window.canvasRenderer.SetAlpha(0);

        ShowPopup(3);
    }

    public void UpdateWindow(Texture icon, string text)
    {
        windowIcon.texture = icon; 
        windowText.text = text;
    }

    public void ShowPopup(float s = 3f)
    {
        StartCoroutine(FadePopup(s));
    }

    private IEnumerator FadePopup(float s)
    {
        Debug.Log("ShowPopup starts");

        showPopup = true;
        // Splits into 3 parts
        float part = s / 3f;

        Window.canvasRenderer.SetAlpha(0f);
        windowIcon.canvasRenderer.SetAlpha(0f);
        windowText.canvasRenderer.SetAlpha(0f);

        // Fade in
        Window.CrossFadeAlpha(1f, part, false);
        windowIcon.CrossFadeAlpha(1f, part, false);
        windowText.CrossFadeAlpha(1f, part, false);

        yield return new WaitForSeconds(part + part);

        Window.CrossFadeAlpha(0f, part, false);
        windowIcon.CrossFadeAlpha(0f, part, false);
        windowText.CrossFadeAlpha(0f, part, false);

        yield return new WaitForSeconds(part);

        showPopup = false;

        Debug.Log("ShowPopup ends");
    }
    
}
