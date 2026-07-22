using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Startup : MonoBehaviour
{
    [SerializeField, Range(0, 1)] float x_pos = 0.05f;
    [SerializeField, Range(0, 1)] float y_pos = 0.1f;
    [SerializeField, Range(0, 1)] float width = 0.3f;
    [SerializeField, Range(0, 1)] float height = 0.02f;
    [SerializeField] GameObject BG;
    [SerializeField] GameObject Current;
    [SerializeField] GameObject Square;
    // Default origin is top left, y is inverted 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RectTransform rectTransform = transform as RectTransform;
        rectTransform.position = new Vector3(Screen.width * x_pos, Screen.height * (1 - y_pos), 0);
        rectTransform.sizeDelta = new Vector2(Screen.width * width, Screen.height * height);

        RectTransform bgRectTransform = BG.transform as RectTransform;
        bgRectTransform.anchorMin = new Vector2(0, 0);
        bgRectTransform.anchorMax = new Vector2(1, 1);
        bgRectTransform.offsetMax = new Vector2(0, 0); //L, B
        bgRectTransform.offsetMin = new Vector2(0, 0); //R, T

        RectTransform currentRectTransform = Current.transform as RectTransform;
        currentRectTransform.anchorMin = new Vector2(0, 0);
        currentRectTransform.anchorMax = new Vector2(1, 1);
        currentRectTransform.offsetMax = new Vector2(0, 0); //L, B
        currentRectTransform.offsetMin = new Vector2(0, 0); //R, T

        Image CurrentImage = Current.GetComponent<Image>();
        CurrentImage.type = Image.Type.Filled;
        CurrentImage.fillMethod = Image.FillMethod.Horizontal;
        CurrentImage.fillOrigin = (int)Image.OriginHorizontal.Left;

        RectTransform SquareTransform = Square.transform as RectTransform;
        SquareTransform.localScale = new Vector3(Screen.width / 2560f, Screen.height / 1440f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
