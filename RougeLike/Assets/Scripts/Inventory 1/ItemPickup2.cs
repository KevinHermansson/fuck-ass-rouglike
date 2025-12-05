using UnityEngine;
using TMPro;
using System.Linq;

public class ItemPickup2 : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ItemType2 itemToGive;

    public void SetItem(ItemType2 item)
    {
        itemToGive = item;
    }

    [Header("Pickup Settings")]
    [SerializeField] private float pickupDistance = 2f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private bool autoPickup = false;

    [Header("Time Limit")]
    [SerializeField] private bool hasTimeLimit = false;
    [SerializeField] private float timeLimitSeconds = 60f;

    [Header("Font (Optional)")]
    [SerializeField] private TMPro.TMP_FontAsset pixelFont;
    
    [Header("Text Outline")]
    [SerializeField] private float outlineWidth = 0.2f;
    [SerializeField] private Color outlineColor = Color.black;
    
    [Header("Animation")]
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobAmount = 10f;

    private Transform player;
    private bool isPickedUp = false;
    private bool isInRange = false;
    private PickupPrompt pickupPrompt;
    private float timeRemaining = 0f;
    private GameObject timerObj;
    
    private static ItemPickup2 activePickup = null;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (itemToGive == null)
        {
            Debug.LogWarning($"ItemPickup2 on {gameObject.name} has no item assigned!");
        }

        if (hasTimeLimit)
        {
            timeRemaining = timeLimitSeconds;
        }

        CreatePromptUI();
    }

    private void CreatePromptUI()
    {
        Canvas canvas = GetOrCreateCanvas();
        TMPro.TMP_FontAsset font = GetFontAsset();
        
        timerObj = CreateTimerText(canvas, font);
        GameObject promptObj = CreatePromptText(canvas, font);
        
        pickupPrompt = promptObj.AddComponent<PickupPrompt>();
        pickupPrompt.Initialize(
            promptObj.GetComponentInChildren<TextMeshProUGUI>(),
            timerObj.GetComponent<TextMeshProUGUI>(),
            promptObj,
            timerObj,
            bobSpeed,
            bobAmount
        );
        
        promptObj.SetActive(false);
        if (timerObj != null)
        {
            timerObj.SetActive(hasTimeLimit);
        }
    }

    private Canvas GetOrCreateCanvas()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("PickupPromptCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        return canvas;
    }

    private TMPro.TMP_FontAsset GetFontAsset()
    {
        if (pixelFont != null)
            return pixelFont;

        TMPro.TMP_FontAsset font = Resources.FindObjectsOfTypeAll<TMPro.TMP_FontAsset>()
            .FirstOrDefault(f => f.name.Contains("Thaleah", System.StringComparison.OrdinalIgnoreCase));
        
        if (font == null)
        {
            font = UnityEngine.Resources.Load<TMPro.TMP_FontAsset>("ThaleahFat_TTF SDF");
        }
        
        if (font == null)
        {
            font = Resources.FindObjectsOfTypeAll<TMPro.TMP_FontAsset>()
                .FirstOrDefault(f => f.name.Contains("ThaleahFat", System.StringComparison.OrdinalIgnoreCase));
        }

        return font ?? TMPro.TMP_Settings.defaultFontAsset;
    }

    private GameObject CreateTimerText(Canvas canvas, TMPro.TMP_FontAsset font)
    {
        GameObject timerObj = new GameObject("TimerText");
        timerObj.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = timerObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
        
        TextMeshProUGUI timerText = timerObj.AddComponent<TextMeshProUGUI>();
        timerText.text = "00:00";
        timerText.font = font;
        timerText.fontSize = 40;
        timerText.alignment = TextAlignmentOptions.Center;
        timerText.color = Color.yellow;
        timerText.outlineWidth = outlineWidth;
        timerText.outlineColor = outlineColor;
        
        return timerObj;
    }

    private GameObject CreatePromptText(Canvas canvas, TMPro.TMP_FontAsset font)
    {
        GameObject promptObj = new GameObject("PickupPrompt");
        promptObj.transform.SetParent(canvas.transform, false);
        
        RectTransform rect = promptObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 50);
        
        GameObject textObj = new GameObject("PromptText");
        textObj.transform.SetParent(promptObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI promptText = textObj.AddComponent<TextMeshProUGUI>();
        promptText.text = $"Press {pickupKey} to pick up";
        promptText.font = font;
        promptText.fontSize = 24;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.color = Color.white;
        promptText.outlineWidth = outlineWidth;
        promptText.outlineColor = outlineColor;
        
        return promptObj;
    }

    private void Update()
    {
        if (isPickedUp || itemToGive == null || player == null) return;

        if (hasTimeLimit)
        {
            timeRemaining -= Time.deltaTime;
            
            if (timeRemaining <= 0f)
            {
                HidePrompt();
                Destroy(gameObject);
                return;
            }
        }

        float distance = Vector2.Distance(transform.position, player.position);
        bool wasInRange = isInRange;
        isInRange = distance <= pickupDistance;

        if (isInRange && !wasInRange)
        {
            SetAsActivePickup();
        }
        else if (!isInRange && wasInRange)
        {
            if (activePickup == this)
            {
                activePickup = null;
            }
            HidePrompt();
        }
        
        if (isInRange && activePickup == this)
        {
            if (!wasInRange)
            {
                ShowPrompt();
            }
        }
        else if (isInRange && activePickup != this)
        {
            HidePrompt();
        }

        if (pickupPrompt != null)
        {
            if (hasTimeLimit && timerObj != null && timerObj.activeSelf)
            {
                pickupPrompt.UpdateTimer(timeRemaining);
            }
            
            bool showPrompt = (activePickup == this && isInRange);
            pickupPrompt.UpdatePosition(transform.position, showPrompt, hasTimeLimit);
        }

        if (isInRange && activePickup == this)
        {
            if (autoPickup || Input.GetKeyDown(pickupKey))
            {
                TryPickup();
            }
        }
    }

    private void ShowPrompt()
    {
        if (pickupPrompt == null) return;

        string buttonText = autoPickup ? "Picking up..." : $"Press {pickupKey} to pick up";
        bool showTimer = hasTimeLimit;

        pickupPrompt.ShowPrompt(buttonText, showTimer, timeRemaining);
    }

    public void HidePrompt()
    {
        if (pickupPrompt != null)
            pickupPrompt.HidePrompt();
        
        if (timerObj != null)
        {
            timerObj.SetActive(false);
        }
    }
    
    private void SetAsActivePickup()
    {
        if (activePickup != null && activePickup != this)
        {
            activePickup.HidePrompt();
        }
        
        activePickup = this;
    }
    
    private void OnDestroy()
    {
        if (activePickup == this)
        {
            activePickup = null;
        }
    }

    private void TryPickup()
    {
        if (itemToGive == null) return;

        bool success = false;
        string inventoryName = "";

        switch (itemToGive.Category)
        {
            case ItemCategory.Seed:
                if (SeedInventory2.Instance != null)
                {
                    success = SeedInventory2.Instance.AddItem(itemToGive);
                    inventoryName = "Seed";
                }
                break;
            case ItemCategory.Item:
                if (RegularInventory2.Instance != null)
                {
                    success = RegularInventory2.Instance.AddItem(itemToGive);
                    inventoryName = "Regular";
                }
                break;
            case ItemCategory.Weapon:
                if (WeaponInventory2.Instance != null)
                {
                    success = WeaponInventory2.Instance.AddItem(itemToGive);
                    inventoryName = "Weapon";
                }
                break;
        }

        if (success)
        {
            Debug.Log($"Picked up: {itemToGive.DisplayName} (added to {inventoryName} inventory)");
            
            if (timerObj != null)
            {
                timerObj.SetActive(false);
                Destroy(timerObj);
            }
            if (pickupPrompt != null)
            {
                pickupPrompt.HidePrompt();
            }
            
            isPickedUp = true;
            Destroy(gameObject);
        }
        else
        {
            HidePrompt();
            ShowReplacementUI(itemToGive);
        }
    }


    private void ShowReplacementUI(ItemType2 item)
    {
        if (ItemReplacementUI.Instance == null)
        {
            Debug.LogWarning("ItemReplacementUI.Instance is null!");
            return;
        }

        InventoryUI2 targetUI = null;
        InventorySlotButton[] slotButtons = null;

        GameObject replacementPanelObj = ItemReplacementUI.Instance?.ReplacementPanel;
        Transform searchRoot = null;
        
        if (replacementPanelObj != null)
        {
            Canvas panelCanvas = replacementPanelObj.GetComponent<Canvas>();
            if (panelCanvas != null)
            {
                searchRoot = replacementPanelObj.transform;
            }
            else
            {
                Canvas parentCanvas = replacementPanelObj.GetComponentInParent<Canvas>();
                if (parentCanvas != null)
                {
                    searchRoot = parentCanvas.transform;
                }
                else
                {
                    searchRoot = replacementPanelObj.transform;
                }
            }
        }
        
        if (searchRoot == null)
        {
            GameObject replacementCanvas = GameObject.Find("ReplacementCanvas");
            if (replacementCanvas != null)
            {
                searchRoot = replacementCanvas.transform;
            }
        }

        switch (item.Category)
        {
            case ItemCategory.Seed:
                if (searchRoot != null)
                {
                    Transform seedViewTransform = searchRoot.Find("Menu")?.Find("SeedView");
                    if (seedViewTransform == null)
                    {
                        seedViewTransform = FindChildRecursive(searchRoot, "SeedView");
                    }
                    if (seedViewTransform != null)
                    {
                        targetUI = seedViewTransform.GetComponent<InventoryUI2>();
                    }
                }
                
                if (targetUI == null)
                {
                    GameObject seedView = GameObject.Find("SeedView");
                    if (seedView == null)
                    {
                        Transform seedViewTransform = GameObject.Find("InventoryCanvas")?.transform?.Find("Menu")?.Find("SeedView");
                        if (seedViewTransform != null) seedView = seedViewTransform.gameObject;
                    }
                    if (seedView != null)
                    {
                        targetUI = seedView.GetComponent<InventoryUI2>();
                    }
                }
                
                if (targetUI == null)
                {
                    InventoryUI2[] allUIs = FindObjectsByType<InventoryUI2>(FindObjectsSortMode.None);
                    foreach (InventoryUI2 ui in allUIs)
                    {
                        var inventoryTypeField = typeof(InventoryUI2).GetField("inventoryType", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (inventoryTypeField != null)
                        {
                            ItemCategory type = (ItemCategory)inventoryTypeField.GetValue(ui);
                            if (type == ItemCategory.Seed)
                            {
                                targetUI = ui;
                                break;
                            }
                        }
                    }
                }
                break;

            case ItemCategory.Item:
                if (searchRoot != null)
                {
                    Transform itemViewTransform = searchRoot.Find("Menu")?.Find("ItemView");
                    if (itemViewTransform == null)
                    {
                        itemViewTransform = FindChildRecursive(searchRoot, "ItemView");
                    }
                    if (itemViewTransform != null)
                    {
                        targetUI = itemViewTransform.GetComponent<InventoryUI2>();
                    }
                }
                
                if (targetUI == null)
                {
                    GameObject itemView = GameObject.Find("ItemView");
                    if (itemView == null)
                    {
                        Transform itemViewTransform = GameObject.Find("InventoryCanvas")?.transform?.Find("Menu")?.Find("ItemView");
                        if (itemViewTransform != null) itemView = itemViewTransform.gameObject;
                    }
                    if (itemView != null)
                    {
                        targetUI = itemView.GetComponent<InventoryUI2>();
                    }
                }
                
                if (targetUI == null)
                {
                    InventoryUI2[] allUIs = FindObjectsByType<InventoryUI2>(FindObjectsSortMode.None);
                    foreach (InventoryUI2 ui in allUIs)
                    {
                        var inventoryTypeField = typeof(InventoryUI2).GetField("inventoryType", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (inventoryTypeField != null)
                        {
                            ItemCategory type = (ItemCategory)inventoryTypeField.GetValue(ui);
                            if (type == ItemCategory.Item)
                            {
                                targetUI = ui;
                                break;
                            }
                        }
                    }
                }
                break;

            case ItemCategory.Weapon:
                if (searchRoot != null)
                {
                    Transform hudTransform = searchRoot.Find("HUD");
                    if (hudTransform == null)
                    {
                        hudTransform = FindChildRecursive(searchRoot, "HUD");
                    }
                    if (hudTransform != null)
                    {
                        targetUI = hudTransform.GetComponent<InventoryUI2>();
                    }
                }
                
                if (targetUI == null)
                {
                    GameObject hud = GameObject.Find("HUD");
                    if (hud == null)
                    {
                        Transform hudTransform = GameObject.Find("InventoryCanvas")?.transform?.Find("HUD");
                        if (hudTransform != null) hud = hudTransform.gameObject;
                    }
                    if (hud != null)
                    {
                        targetUI = hud.GetComponent<InventoryUI2>();
                    }
                }
                
                if (targetUI == null)
                {
                    InventoryUI2[] allUIs = FindObjectsByType<InventoryUI2>(FindObjectsSortMode.None);
                    foreach (InventoryUI2 ui in allUIs)
                    {
                        var inventoryTypeField = typeof(InventoryUI2).GetField("inventoryType", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (inventoryTypeField != null)
                        {
                            ItemCategory type = (ItemCategory)inventoryTypeField.GetValue(ui);
                            if (type == ItemCategory.Weapon)
                            {
                                targetUI = ui;
                                break;
                            }
                        }
                    }
                }
                break;
        }

        if (targetUI != null)
        {
            slotButtons = targetUI.GetSlotButtons();
            
            if (slotButtons == null || slotButtons.Length == 0)
            {
                InventorySlotButton[] foundButtons = targetUI.GetComponentsInChildren<InventorySlotButton>();
                if (foundButtons != null && foundButtons.Length > 0)
                {
                    slotButtons = foundButtons;
                }
            }
        }

        if (slotButtons != null && slotButtons.Length > 0)
        {
            ItemReplacementUI.Instance.ShowReplacementUI(item, item.Category, slotButtons, gameObject);
        }
        else
        {
            Debug.LogWarning($"Could not find inventory UI for {item.Category} category! TargetUI: {targetUI != null}, SlotButtons: {slotButtons?.Length ?? 0}");
        }
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
            Transform found = FindChildRecursive(child, name);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupDistance);
    }
}

