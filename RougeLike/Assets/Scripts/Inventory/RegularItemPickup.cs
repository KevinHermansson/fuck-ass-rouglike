using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegularItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public RegularItem itemToGive;
    public GameObject itemPickupPrefab;
    
    [Header("Proximity Settings")]
    [SerializeField] private float proximityDistance = 2f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    public bool useProximityPickup = false;
    
    private Transform player;
    private RegularInventoryHolder playerInventoryHolder;
    private bool isInProximity = false;
    private bool isPickedUp = false;
    private GameObject buttonPromptObj;
    private Canvas worldCanvas;

    private void Start()
    {
        if (useProximityPickup)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerInventoryHolder = playerObj.GetComponent<RegularInventoryHolder>();
                if (playerInventoryHolder == null)
                    playerInventoryHolder = playerObj.GetComponentInParent<RegularInventoryHolder>();
            }
            CreateWorldCanvas();
            CreateButtonPromptUI();
        }
    }

    private void Update()
    {
        if (!useProximityPickup || isPickedUp) return;
        
        if (player == null || playerInventoryHolder == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerInventoryHolder = playerObj.GetComponent<RegularInventoryHolder>();
                if (playerInventoryHolder == null)
                    playerInventoryHolder = playerObj.GetComponentInParent<RegularInventoryHolder>();
            }
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
        
        if (isInProximity != wasInProximity && buttonPromptObj != null)
        {
            buttonPromptObj.SetActive(isInProximity);
        }
    }

    private void AttemptPickup()
    {
        var holder = playerInventoryHolder;
        if (holder == null || holder.Inventory == null || itemToGive == null) return;

        if (holder.Inventory.HasEmptySlot(out int emptyIndex))
        {
            holder.Inventory.AddOrReplace(itemToGive, out ItemBase replaced);
            
            if (replaced != null && itemPickupPrefab != null)
            {
                var go = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
                var pickup = go.GetComponent<RegularItemPickup>();
                if (pickup != null) pickup.itemToGive = replaced as RegularItem;
            }
            
            isPickedUp = true;
            CleanupUI();
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
            replacementUI.ShowReplacementPrompt(itemToGive, this, false);
        }
    }

    public void ConfirmReplacement(int slotIndex)
    {
        if (playerInventoryHolder == null || playerInventoryHolder.Inventory == null) return;
        
        var replaced = playerInventoryHolder.Inventory.GetAt(slotIndex);
        
        playerInventoryHolder.Inventory.RemoveAt(slotIndex);
        playerInventoryHolder.Inventory.AddOrReplace(itemToGive, out ItemBase _);
        
        if (replaced != null && itemPickupPrefab != null)
        {
            var go = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
            var pickup = go.GetComponent<RegularItemPickup>();
            if (pickup != null) pickup.itemToGive = replaced as RegularItem;
        }
        
        isPickedUp = true;
        CleanupUI();
        Destroy(gameObject);
    }
    
    private void CleanupUI()
    {
        if (buttonPromptObj != null) Destroy(buttonPromptObj);
        if (worldCanvas != null) Destroy(worldCanvas.gameObject);
    }
    
    private void OnDestroy()
    {
        CleanupUI();
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
    
    private void CreateButtonPromptUI()
    {
        if (worldCanvas == null) return;
        
        GameObject promptObj = new GameObject("ButtonPrompt");
        promptObj.transform.SetParent(worldCanvas.transform, false);
        promptObj.SetActive(false);
        
        RectTransform rect = promptObj.AddComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 0.3f);
        rect.sizeDelta = new Vector2(200, 50);
        
        TextMeshProUGUI promptText = promptObj.AddComponent<TextMeshProUGUI>();
        promptText.text = $"[{pickupKey}] Pickup";
        promptText.fontSize = 24;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.color = Color.yellow;
        
        buttonPromptObj = promptObj;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (useProximityPickup) return;
        
        var holder = other.GetComponentInParent<RegularInventoryHolder>();
        if (holder == null || holder.Inventory == null || itemToGive == null) return;

        holder.Inventory.AddOrReplace(itemToGive, out ItemBase replaced);

        if (replaced != null && itemPickupPrefab != null)
        {
            var go = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
            var pickup = go.GetComponent<RegularItemPickup>();
            if (pickup != null) pickup.itemToGive = replaced as RegularItem;
        }

        CleanupUI();
        Destroy(gameObject);
    }
}
