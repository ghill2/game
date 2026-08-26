using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupData
{
    public Texture icon;
    public string text;

    public PopupData(Texture icon, string text)
    {
        this.icon = icon;
        this.text = text;
    }
}

public class PopupController : MonoBehaviour
{
    [SerializeField]
    private Image Window;
    [SerializeField]
    private RawImage windowIcon;
    [SerializeField]
    private TextMeshProUGUI windowText;

    // Next Popup Storage
    private Queue<PopupData> popup_Queue;

    private bool showingPopup = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Window = GetComponent<Image>();
        windowIcon = GetComponentInChildren<RawImage>();
        windowText = GetComponentInChildren<TextMeshProUGUI>();

        popup_Queue = new Queue<PopupData>();

        Window.canvasRenderer.SetAlpha(0f);
        windowIcon.canvasRenderer.SetAlpha(0f);
        windowText.canvasRenderer.SetAlpha(0f);
    }

    public void UpdateWindow(Texture icon, string text)
    { 
        popup_Queue.Enqueue(new PopupData(icon, text));
        if (!showingPopup && popup_Queue.TryDequeue(out PopupData data))
        {
            windowIcon.texture = data.icon;
            windowText.text = data.text;
            ShowPopup();
        }
    }

    private void ShowPopup(float s = 3f)
    {
        if (!showingPopup)
        {
            StartCoroutine(FadePopup(s));
        }
    }

    private IEnumerator FadePopup(float s)
    {
        Debug.Log("ShowPopup starts");

        showingPopup = true;
        // Splits into 3 parts
        float part = s / 3f;

        Window.canvasRenderer.SetAlpha(0f);
        windowIcon.canvasRenderer.SetAlpha(0f);
        windowText.canvasRenderer.SetAlpha(0f);

        // Fade in
        Window.CrossFadeAlpha(1f, part, false);
        windowIcon.CrossFadeAlpha(1f, part, false);
        windowText.CrossFadeAlpha(1f, part, false);

        yield return new WaitForSeconds(part);

        // Show for (s / 3) seconds;
        yield return new WaitForSeconds(part); 

        // Fade out
        Window.CrossFadeAlpha(0f, part, false);
        windowIcon.CrossFadeAlpha(0f, part, false);
        windowText.CrossFadeAlpha(0f, part, false);

        yield return new WaitForSeconds(part);

        showingPopup = false;

        Debug.Log("ShowPopup ends");

        // Call itself again if there's Popup queued
        if (!showingPopup && popup_Queue.Count > 0)
        {
            if (popup_Queue.TryDequeue(out PopupData data))
            {
                Debug.Log($"NextIcon: {data.icon}, NextText: {data.text}");
                UpdateWindow(data.icon, data.text);
                ShowPopup();
            }
            else
            {
                Debug.Log($"Dequeue Failed");
            }
        }
    }
}
