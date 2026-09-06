using System.Collections;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    [SerializeField]
    public float duration = 5f;
    private RectTransform rectTransform;

    [SerializeField]
    private float startY;
    [SerializeField]
    private float endY = 1050f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startY = rectTransform.anchoredPosition.y;

        //Test
        StartCoroutine(Scroll());
    }

    IEnumerator Scroll()
    {
        float elapsed = 0f;
        Vector2 startPos = new (rectTransform.anchoredPosition.x, startY);
        Vector2 endPos = new (rectTransform.anchoredPosition.x, endY);

        rectTransform.anchoredPosition = startPos;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        //Snap to end pos
        rectTransform.anchoredPosition = endPos;
    }
}
