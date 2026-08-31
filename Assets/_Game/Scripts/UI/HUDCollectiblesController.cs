using UnityEngine;
using TMPro;
using System.Collections;

public class HUDCollectiblesController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI eggsCountText;

    [SerializeField]
    private GameObject BootsTimer;
    [SerializeField]
    private TextMeshProUGUI TimerText;

    private Coroutine TimerCoroutine;

    public void Start()
    {
        BootsTimer = GameObject.Find("Boots Timer");
        TimerText = BootsTimer.GetComponentInChildren<TextMeshProUGUI>();

        // Test

        UpdateBootsTimer(10);
    }

    public void SetEggsCount(int count)
    {
        eggsCountText.text = $"{count}";
    }

    public void UpdateBootsTimer(float duration)
    {
        if (TimerCoroutine != null)
        {
            StopCoroutine(TimerCoroutine);
        }
        TimerCoroutine = StartCoroutine(BootsTimerCountdown(duration));
    }

    private IEnumerator BootsTimerCountdown(float duration)
    {
        float timer = duration;
        string text;
        int min;
        int sec;

        BootsTimer.SetActive(true);
        while (timer > 0)
        {
            min = Mathf.FloorToInt(timer / 60);
            sec = Mathf.FloorToInt(timer % 60);
            text = $"{min}:{sec:00}";
            TimerText.text = text;
            yield return null; //Waits for next frame

            timer -= Time.deltaTime;
        }
        TimerText.text = "0:00";
        BootsTimer.SetActive(false);

        TimerCoroutine = null;        
    }
}
