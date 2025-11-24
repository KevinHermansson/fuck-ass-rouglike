using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeLimitedSeedPickup : MonoBehaviour
{
    [Header("Seed Settings")]
    [SerializeField] private SeedItem seedToGive;
    [SerializeField] private float lifetimeSeconds = 10f;
    
    [Header("Proximity Settings")]
    [SerializeField] private float proximityDistance = 2f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    
    [Header("UI References")]
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject buttonPromptUI;
    
    private Transform player;
    private SeedInventoryHolder playerInventoryHolder;
    private float timeRemaining;
    private bool isInProximity = false;
    private bool isPickedUp = false;
    
    private GameObject timerTextObj;
    private GameObject buttonPromptObj;
    
    private void Start()
    {
        timeRemaining = lifetimeSeconds;
        
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerInventoryHolder = playerObj.GetComponentInParent<SeedInventoryHolder>();
        }
        
        if (worldCanvas == null)
        {
            CreateWorldCanvas();
        }
        
        CreateTimerUI();
        CreateButtonPromptUI();
    }
    
    private void Update()
    {
        if (isPickedUp) return;
        
        timeRemaining -= Time.deltaTime;
        UpdateTimerDisplay();
        
        if (timeRemaining <= 0f)
        {
            DestroySeed();
            return;
        }
        
        CheckProximity();
        
        if (isInProximity && Input.GetKeyDown(pickupKey))
        {
            AttemptPickup();
        }
    }
    
    private void CheckProximity()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        bool wasInProximity = isInProximity;
        isInProximity = distance <= proximityDistance;
        
        if (isInProximity != wasInProximity)
        {
            if (buttonPromptObj != null)
                buttonPromptObj.SetActive(isInProximity);
        }
        
        if (timerTextObj != null && timerText != null)
        {
            float scale = isInProximity ? 0.7f : 1f;
            timerTextObj.transform.localScale = Vector3.one * scale;
        }
    }
    
    private void AttemptPickup()
    {
        if (playerInventoryHolder == null || playerInventoryHolder.Inventory == null || seedToGive == null)
            return;
        
        if (playerInventoryHolder.Inventory.HasEmptySlot(out int emptyIndex))
        {
            playerInventoryHolder.Inventory.AddOrReplace(seedToGive, out SeedItem replaced);
            isPickedUp = true;
            Destroy(gameObject);
        }
        else
        {
            ShowReplacementUI();
        }
    }
    
    private void ShowReplacementUI()
    {
        var replacementUI = InventoryReplacementUI.Instance;
        if (replacementUI != null)
        {
            replacementUI.ShowReplacementPrompt(seedToGive, this, true);
        }
    }
    
    public void ConfirmReplacement(int slotIndex)
    {
        if (playerInventoryHolder == null || playerInventoryHolder.Inventory == null) return;
        
        var replaced = playerInventoryHolder.Inventory.GetAt(slotIndex);
        playerInventoryHolder.Inventory.RemoveAt(slotIndex);
        playerInventoryHolder.Inventory.AddOrReplace(seedToGive, out SeedItem _);
        
        isPickedUp = true;
        Destroy(gameObject);
    }
    
    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timerText.text = seconds.ToString();
            
            if (timeRemaining <= 3f)
                timerText.color = Color.red;
            else if (timeRemaining <= lifetimeSeconds * 0.5f)
                timerText.color = Color.yellow;
            else
                timerText.color = Color.white;
        }
    }
    
    private void CreateWorldCanvas()
    {
        GameObject canvasObj = new GameObject("WorldCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = Vector3.zero;
        
        worldCanvas = canvasObj.AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.WorldSpace;
        worldCanvas.worldCamera = Camera.main;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        RectTransform rectTransform = canvasObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = Vector3.one * 0.01f;
    }
    
    private void CreateTimerUI()
    {
        if (worldCanvas == null) return;
        
        GameObject timerObj = new GameObject("TimerText");
        timerObj.transform.SetParent(worldCanvas.transform, false);
        
        RectTransform rect = timerObj.AddComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 0.3f);
        rect.sizeDelta = new Vector2(100, 50);
        
        timerText = timerObj.AddComponent<TextMeshProUGUI>();
        timerText.text = lifetimeSeconds.ToString();
        timerText.fontSize = 36;
        timerText.alignment = TextAlignmentOptions.Center;
        timerText.color = Color.white;
        
        timerTextObj = timerObj;
    }
    
    private void CreateButtonPromptUI()
    {
        if (worldCanvas == null) return;
        
        GameObject promptObj = new GameObject("ButtonPrompt");
        promptObj.transform.SetParent(worldCanvas.transform, false);
        promptObj.SetActive(false);
        
        RectTransform rect = promptObj.AddComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -0.2f);
        rect.sizeDelta = new Vector2(200, 50);
        
        TextMeshProUGUI promptText = promptObj.AddComponent<TextMeshProUGUI>();
        promptText.text = $"[{pickupKey}] Pickup";
        promptText.fontSize = 24;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.color = Color.yellow;
        
        buttonPromptObj = promptObj;
    }
    
    private void DestroySeed()
    {
        if (timerTextObj != null) Destroy(timerTextObj);
        if (buttonPromptObj != null) Destroy(buttonPromptObj);
        if (worldCanvas != null) Destroy(worldCanvas.gameObject);
        
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        DestroySeed();
    }
}
