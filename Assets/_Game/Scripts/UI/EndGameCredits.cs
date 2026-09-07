using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class EndGameCredits : MonoBehaviour
{
    [SerializeField] private TMP_Text creditsText;
    [SerializeField, Min(1f)] private float scrollSpeed = 70f;
    [SerializeField, Min(0f)] private float openingPause = 1.5f;
    [SerializeField] private string mainMenuSceneName = "SCN_RC_MainMenu";

    private InputAction returnAction;
    private RectTransform creditsRect;
    private float pauseRemaining;
    private float startY;
    private float endY;
    private float distance;
    private float layoutWidth;
    private bool ready;
    private bool returning;

    private void Awake()
    {
        // Use our own action so this scene does not need the gameplay UI.
        returnAction = InputSystem.actions?.FindAction("Return")?.Clone()
            ?? new InputAction("Return", InputActionType.Button, "<Keyboard>/escape");
        returnAction.AddBinding("<Keyboard>/enter");
        returnAction.AddBinding("<Keyboard>/numpadEnter");

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnEnable()
    {
        returnAction.Enable();
    }

    private void Start()
    {
        if (creditsText == null)
        {
            Debug.LogError("The end-game credits text is not assigned.", this);
            return;
        }

        GameStateManager state = GameStateManager.Instance;
        state?.FinishGame();

        int totalScore = state != null ? state.TotalScore : 0;
        double totalSeconds = state != null ? state.TotalGameTimeSeconds : 0d;
        long seconds = (long)System.Math.Floor(totalSeconds);
        string totalTime = $"{seconds / 3600:00}:{seconds / 60 % 60:00}:{seconds % 60:00}";
        creditsText.text = $"Total Score: {totalScore}\nTotal Time: {totalTime}\n\n{creditsText.text}";

        creditsRect = creditsText.rectTransform;
        pauseRemaining = openingPause;
        Canvas.ForceUpdateCanvases();
        UpdateLayout();
        ready = true;
    }

    private void Update()
    {
        if (returnAction.WasPressedThisFrame())
        {
            ReturnToMainMenu();
            return;
        }

        if (!ready || returning)
        {
            return;
        }

        if (!Mathf.Approximately(layoutWidth, creditsRect.rect.width))
        {
            UpdateLayout();
        }

        if (pauseRemaining > 0f)
        {
            pauseRemaining -= Time.unscaledDeltaTime;
            return;
        }

        distance = Mathf.Min(distance + scrollSpeed * Time.unscaledDeltaTime, endY - startY);
        creditsRect.anchoredPosition = new Vector2(0f, startY + distance);
    }

    private void UpdateLayout()
    {
        float progress = endY > startY ? distance / (endY - startY) : 0f;
        layoutWidth = creditsRect.rect.width;
        creditsRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, creditsText.preferredHeight);
        creditsText.ForceMeshUpdate();

        TMP_TextInfo info = creditsText.textInfo;
        if (info.lineCount == 0)
        {
            return;
        }

        TMP_LineInfo firstLine = info.lineInfo[0];
        TMP_LineInfo lastLine = info.lineInfo[info.lineCount - 1];

        // Center the firrst line
        startY = -(firstLine.ascender + firstLine.descender) * 0.5f;
        endY = Mathf.Max(startY, -(lastLine.ascender + lastLine.descender) * 0.5f);
        distance = progress * (endY - startY);
        creditsRect.anchoredPosition = new Vector2(0f, startY + distance);
    }

    private void ReturnToMainMenu()
    {
        if (returning)
        {
            return;
        }

        returning = true;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(mainMenuSceneName, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        returnAction?.Disable();
    }

    private void OnDestroy()
    {
        returnAction?.Dispose();
    }
}
