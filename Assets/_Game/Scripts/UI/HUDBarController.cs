using UnityEngine;
using UnityEngine.UI;

public class HUDBarController : MonoBehaviour
{
    [SerializeField] private RectTransform fillRect;

    private void Awake()
    {
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
    }

    public void UpdateBar(float percentage)
    {
        Vector2 anchorMax = fillRect.anchorMax;
        anchorMax.x = percentage;
        fillRect.anchorMax = anchorMax;
    }
}
