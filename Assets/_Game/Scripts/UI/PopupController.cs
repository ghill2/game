using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PopupController : MonoBehaviour
{
    [SerializeField]
    private Image Window;
    [SerializeField]
    private RawImage windowIcon;
    [SerializeField]
    private TextMeshProUGUI windowText;

    // Next Popup Storage
    private Queue<Texture> Icon_Queue;
    private Queue<string> Text_Queue;

    private bool showingPopup = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Window = GetComponent<Image>();
        windowIcon = GetComponentInChildren<RawImage>();
        windowText = GetComponentInChildren<TextMeshProUGUI>();

        Icon_Queue = new Queue<Texture>();
        Text_Queue = new Queue<string>();

        Window.canvasRenderer.SetAlpha(0f);
        windowIcon.canvasRenderer.SetAlpha(0f);
        windowText.canvasRenderer.SetAlpha(0f);
    }

    public void UpdateWindow(Texture icon, string text)
    {
        if (showingPopup) // There's an Ongoing popup
        {
            Icon_Queue.Enqueue(icon);
            Text_Queue.Enqueue(text);
        }
        else //Show popup instantly
        {
            windowIcon.texture = icon;
            windowText.text = text;
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
        if (!showingPopup && Icon_Queue.Count > 0)
        {
            if (Icon_Queue.TryDequeue(out Texture NextIcon) && Text_Queue.TryDequeue(out string NextText))
            {
                Debug.Log($"NextIcon: {NextIcon}, NextText: {NextText}");
                UpdateWindow(NextIcon, NextText);
                ShowPopup();
            }
            else
            {
                Debug.Log($"Dequeue Failed");
            }
        }
    }
}
