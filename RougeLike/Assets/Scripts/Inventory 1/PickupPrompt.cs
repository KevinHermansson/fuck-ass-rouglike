using UnityEngine;
using TMPro;

public class PickupPrompt : MonoBehaviour
{
    private TextMeshProUGUI promptText;
    private TextMeshProUGUI timerText;
    private GameObject promptPanel;
    private GameObject timerPanel;

    private Camera mainCamera;
    private RectTransform canvasRect;
    private Canvas canvas;
    
    private float bobOffset = 0f;
    private float time = 0f;
    private float bobSpeed = 2f;
    private float bobAmount = 10f;

    public void Initialize(TextMeshProUGUI prompt, TextMeshProUGUI timer, GameObject panel, GameObject timerPanelObj, float speed, float amount)
    {
        promptText = prompt;
        timerText = timer;
        promptPanel = panel;
        timerPanel = timerPanelObj;
        bobSpeed = speed;
        bobAmount = amount;
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();

        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindFirstObjectByType<Canvas>();

        if (canvas != null)
            canvasRect = canvas.GetComponent<RectTransform>();

        if (promptPanel == null)
            promptPanel = gameObject;
        if (promptText == null)
            promptText = transform.Find("PromptText")?.GetComponent<TextMeshProUGUI>();
        if (timerText == null)
            timerText = transform.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
    }

    public void ShowPrompt(string buttonText, bool showTimer = false, float timeRemaining = 0f)
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText != null)
            promptText.text = buttonText;

        if (timerText != null && showTimer)
        {
            UpdateTimer(timeRemaining);
        }
    }

    public void UpdateTimer(float timeRemaining)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void HidePrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    public void UpdatePosition(Vector3 worldPosition, bool showPrompt, bool hasTimer = false)
    {
        if (canvas == null || canvasRect == null || mainCamera == null) return;

        time += Time.deltaTime * bobSpeed;
        bobOffset = Mathf.Sin(time) * bobAmount;

        Vector2 screenPoint = mainCamera.WorldToScreenPoint(worldPosition + Vector3.up * 1.5f);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPoint, canvas.worldCamera, out Vector2 localPoint);

        if (timerPanel != null)
        {
            timerPanel.SetActive(hasTimer);
            if (hasTimer)
            {
                RectTransform timerRect = timerPanel.GetComponent<RectTransform>();
                float timerY = showPrompt ? localPoint.y + 25f + bobOffset : localPoint.y + bobOffset;
                timerRect.localPosition = new Vector2(localPoint.x, timerY);
            }
        }

        if (promptPanel != null)
        {
            RectTransform promptRect = promptPanel.GetComponent<RectTransform>();
            if (showPrompt)
            {
                promptRect.localPosition = new Vector2(localPoint.x, localPoint.y - 25f + bobOffset);
            }
            else
            {
                promptRect.localPosition = new Vector2(localPoint.x, localPoint.y - 1000f);
            }
        }
    }
}

