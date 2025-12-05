using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemReplacementUI : MonoBehaviour
{
    [Header("UI References (Optional - leave empty to use existing inventory UI)")]
    [SerializeField] private GameObject replacementPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    
    public GameObject ReplacementPanel => replacementPanel;
    
    [Header("Inventory UI References (for transparency)")]
    [SerializeField] private GameObject seedInventoryUI;
    [SerializeField] private GameObject itemInventoryUI;
    [SerializeField] private GameObject weaponInventoryUI;
    
    [Header("Transparency Settings")]
    [SerializeField] private float inactiveAlpha = 0.5f; // Alpha for non-relevant inventories
    
    private InventorySlotButton[] slotButtons;
    
    private ItemType2 itemToAdd;
    private ItemCategory inventoryCategory;
    private System.Action<int> onSlotSelected;
    private GameObject pickupObject;
    
    private float seedOriginalAlpha = 1f;
    private float itemOriginalAlpha = 1f;
    private float weaponOriginalAlpha = 1f;
    
    public static ItemReplacementUI Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        if (replacementPanel != null)
        {
            replacementPanel.SetActive(false);
        }
        
        FindInventoryUIs();
    }
    
    private void FindInventoryUIs()
    {
        Transform searchRoot = null;
        if (replacementPanel != null)
        {
            Canvas panelCanvas = replacementPanel.GetComponent<Canvas>();
            if (panelCanvas != null)
            {
                searchRoot = replacementPanel.transform;
            }
            else
            {
                Canvas parentCanvas = replacementPanel.GetComponentInParent<Canvas>();
                if (parentCanvas != null)
                {
                    searchRoot = parentCanvas.transform;
                }
                else
                {
                    searchRoot = replacementPanel.transform;
                }
            }
        }
        
        if (seedInventoryUI == null)
        {
            if (searchRoot != null)
            {
                Transform seedViewTransform = FindChildRecursive(searchRoot, "SeedView");
                if (seedViewTransform != null)
                {
                    seedInventoryUI = seedViewTransform.gameObject;
                }
            }
            
            if (seedInventoryUI == null)
            {
                GameObject seedView = GameObject.Find("SeedView");
                if (seedView == null)
                {
                    Transform seedViewTransform = GameObject.Find("InventoryCanvas")?.transform?.Find("Menu")?.Find("SeedView");
                    if (seedViewTransform != null) seedInventoryUI = seedViewTransform.gameObject;
                }
                else
                {
                    seedInventoryUI = seedView;
                }
            }
        }
        
        if (itemInventoryUI == null)
        {
            if (searchRoot != null)
            {
                Transform itemViewTransform = FindChildRecursive(searchRoot, "ItemView");
                if (itemViewTransform != null)
                {
                    itemInventoryUI = itemViewTransform.gameObject;
                }
            }
            
            if (itemInventoryUI == null)
            {
                GameObject itemView = GameObject.Find("ItemView");
                if (itemView == null)
                {
                    Transform itemViewTransform = GameObject.Find("InventoryCanvas")?.transform?.Find("Menu")?.Find("ItemView");
                    if (itemViewTransform != null) itemInventoryUI = itemViewTransform.gameObject;
                }
                else
                {
                    itemInventoryUI = itemView;
                }
            }
        }
        
        if (weaponInventoryUI == null)
        {
            if (searchRoot != null)
            {
                Transform hudTransform = FindChildRecursive(searchRoot, "HUD");
                if (hudTransform != null)
                {
                    weaponInventoryUI = hudTransform.gameObject;
                }
            }
            
            if (weaponInventoryUI == null)
            {
                GameObject hud = GameObject.Find("HUD");
                if (hud == null)
                {
                    Transform hudTransform = GameObject.Find("InventoryCanvas")?.transform?.Find("HUD");
                    if (hudTransform != null) weaponInventoryUI = hudTransform.gameObject;
                }
                else
                {
                    weaponInventoryUI = hud;
                }
            }
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
    
    public void ShowReplacementUI(ItemType2 newItem, ItemCategory category, InventorySlotButton[] slots, GameObject pickup = null)
    {
        itemToAdd = newItem;
        inventoryCategory = category;
        slotButtons = slots;
        pickupObject = pickup;
        
        Debug.Log($"ShowReplacementUI called for {newItem.DisplayName}, category: {category}, slots: {slots?.Length ?? 0}");
        
        if (replacementPanel != null)
        {
            Debug.Log($"Replacement panel found: {replacementPanel.name}, active: {replacementPanel.activeSelf}");
            replacementPanel.SetActive(true);
            
            Canvas canvas = replacementPanel.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.enabled = true;
                canvas.sortingOrder = 100;
                Debug.Log($"Canvas found on replacement panel, enabled: {canvas.enabled}, sortingOrder: {canvas.sortingOrder}");
            }
            else
            {
                canvas = replacementPanel.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    canvas.enabled = true;
                    canvas.sortingOrder = 100;
                    Debug.Log($"Canvas found in parent, enabled: {canvas.enabled}, sortingOrder: {canvas.sortingOrder}");
                }
                else
                {
                    Debug.LogWarning("No Canvas found on replacement panel or its parents!");
                }
            }
            
            SetChildrenActive(replacementPanel.transform, true);
            Debug.Log($"Replacement panel and children activated");
        }
        else
        {
            Debug.LogWarning("Replacement panel is null! Make sure it's assigned in the Inspector.");
        }
        
        FindInventoryUIs();
        
        if (promptText != null)
        {
            promptText.text = $"Inventory full! Choose a slot to replace with {newItem.DisplayName}:";
        }
        
        ApplyTransparency(category);
        
        if (slotButtons != null)
        {
            for (int i = 0; i < slotButtons.Length; i++)
            {
                if (slotButtons[i] != null)
                {
                    Button btn = slotButtons[i].GetComponent<Button>();
                    if (btn != null)
                    {
                        int slotIndex = i;
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => OnSlotSelectedForReplacement(slotIndex));
                    }
                }
            }
        }
    }
    
    private void ApplyTransparency(ItemCategory fullCategory)
    {
        if (seedInventoryUI != null)
        {
            seedOriginalAlpha = GetUIAlpha(seedInventoryUI);
        }
        if (itemInventoryUI != null)
        {
            itemOriginalAlpha = GetUIAlpha(itemInventoryUI);
        }
        if (weaponInventoryUI != null)
        {
            weaponOriginalAlpha = GetUIAlpha(weaponInventoryUI);
        }
        
        switch (fullCategory)
        {
            case ItemCategory.Seed:
                SetUIAlpha(itemInventoryUI, inactiveAlpha);
                SetUIAlpha(weaponInventoryUI, inactiveAlpha);
                SetUIAlpha(seedInventoryUI, 1f);
                break;
                
            case ItemCategory.Item:
                SetUIAlpha(seedInventoryUI, inactiveAlpha);
                SetUIAlpha(weaponInventoryUI, inactiveAlpha);
                SetUIAlpha(itemInventoryUI, 1f);
                break;
                
            case ItemCategory.Weapon:
                SetUIAlpha(seedInventoryUI, inactiveAlpha);
                SetUIAlpha(itemInventoryUI, inactiveAlpha);
                SetUIAlpha(weaponInventoryUI, 1f);
                break;
        }
    }
    
    private float GetUIAlpha(GameObject uiObject)
    {
        if (uiObject == null) return 1f;
        
        CanvasGroup canvasGroup = uiObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            return canvasGroup.alpha;
        }
        
        Image image = uiObject.GetComponent<Image>();
        if (image != null)
        {
            return image.color.a;
        }
        
        return 1f;
    }
    
    private void SetUIAlpha(GameObject uiObject, float alpha)
    {
        if (uiObject == null) return;
        
        CanvasGroup canvasGroup = uiObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = uiObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = alpha;
        
        Image image = uiObject.GetComponent<Image>();
        if (image != null)
        {
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }
    }
    
    private void SetChildrenActive(Transform parent, bool active)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(active);
            SetChildrenActive(child, active);
        }
    }
    
    public void HideReplacementUI()
    {
        if (replacementPanel != null)
        {
            replacementPanel.SetActive(false);
        }
        
        RestoreTransparency();
        
        if (slotButtons != null)
        {
            foreach (var slot in slotButtons)
            {
                if (slot != null)
                {
                    Button btn = slot.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(slot.OnButtonClicked);
                    }
                }
            }
        }
    }
    
    private void RestoreTransparency()
    {
        SetUIAlpha(seedInventoryUI, seedOriginalAlpha);
        SetUIAlpha(itemInventoryUI, itemOriginalAlpha);
        SetUIAlpha(weaponInventoryUI, weaponOriginalAlpha);
    }
    
    private void OnSlotSelectedForReplacement(int slotIndex)
    {
        if (itemToAdd == null) return;
        
        switch (inventoryCategory)
        {
            case ItemCategory.Seed:
                if (SeedInventory2.Instance != null)
                {
                    ItemType2 oldItem = SeedInventory2.Instance.GetAt(slotIndex);
                    if (oldItem != null)
                    {
                        SeedInventory2.Instance.ReplaceAt(slotIndex, itemToAdd);
                    }
                }
                break;
                
            case ItemCategory.Item:
                if (RegularInventory2.Instance != null)
                {
                    ItemType2 oldItem = RegularInventory2.Instance.GetAt(slotIndex);
                    if (oldItem != null)
                    {
                        ItemDropManager dropManager = FindFirstObjectByType<ItemDropManager>();
                        if (dropManager != null)
                        {
                            dropManager.DropItemAtSlot(oldItem, ItemCategory.Item, slotIndex);
                        }
                        RegularInventory2.Instance.ReplaceAt(slotIndex, itemToAdd);
                    }
                }
                break;
                
            case ItemCategory.Weapon:
                if (WeaponInventory2.Instance != null)
                {
                    ItemType2 oldItem = WeaponInventory2.Instance.GetAt(slotIndex);
                    if (oldItem != null)
                    {
                        ItemDropManager dropManager = FindFirstObjectByType<ItemDropManager>();
                        if (dropManager != null)
                        {
                            dropManager.DropItemAtSlot(oldItem, ItemCategory.Weapon, slotIndex);
                        }
                        WeaponInventory2.Instance.ReplaceAt(slotIndex, itemToAdd);
                    }
                }
                break;
        }
        
        if (pickupObject != null)
        {
            ItemPickup2 pickup = pickupObject.GetComponent<ItemPickup2>();
            if (pickup != null)
            {
                pickup.HidePrompt();
            }
            Destroy(pickupObject);
        }
        
        HideReplacementUI();
    }
}

