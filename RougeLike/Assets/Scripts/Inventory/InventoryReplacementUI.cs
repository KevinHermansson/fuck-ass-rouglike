using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryReplacementUI : MonoBehaviour
{
    private static InventoryReplacementUI instance;
    public static InventoryReplacementUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<InventoryReplacementUI>(FindObjectsInactive.Include);
            }
            return instance;
        }
    }
    
    [Header("UI References")]
    [SerializeField] private GameObject replacementPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Button[] slotButtons = new Button[9];
    [SerializeField] private Image[] slotIcons = new Image[9];
    [SerializeField] private TextMeshProUGUI[] slotLabels = new TextMeshProUGUI[9];
    
    private SeedInventory seedInventory;
    private RegularInventory regularInventory;
    private ItemBase itemToAdd;
    private TimeLimitedSeedPickup seedPickup;
    private RegularItemPickup regularPickup;
    private bool isSeedReplacement;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
        }
    }
    
    private void Start()
    {
        var seedHolder = FindFirstObjectByType<SeedInventoryHolder>();
        var regularHolder = FindFirstObjectByType<RegularInventoryHolder>();
        
        if (seedHolder != null) seedInventory = seedHolder.Inventory;
        if (regularHolder != null) regularInventory = regularHolder.Inventory;
        
        if (replacementPanel != null)
        {
            replacementPanel.SetActive(false);
        }
        
        if (promptText != null && promptText.gameObject != replacementPanel)
        {
            promptText.gameObject.SetActive(false);
        }
        
        SetupSlotButtons();
    }
    
    private void Update()
    {
        if (replacementPanel != null && replacementPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CancelReplacement();
        }
    }
    
    private void SetupSlotButtons()
    {
        for (int i = 0; i < slotButtons.Length && i < slotIcons.Length; i++)
        {
            int slotIndex = i;
            if (slotButtons[i] != null)
            {
                slotButtons[i].onClick.RemoveAllListeners();
                slotButtons[i].onClick.AddListener(() => OnSlotSelected(slotIndex));
                
                slotButtons[i].interactable = true;
                slotButtons[i].enabled = true;
                
                var buttonImage = slotButtons[i].GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.raycastTarget = true;
                }
            }
        }
    }
    
    public void ShowReplacementPrompt(ItemBase item, MonoBehaviour pickupScript, bool isSeed)
    {
        itemToAdd = item;
        isSeedReplacement = isSeed;
        
        if (isSeed)
        {
            seedPickup = pickupScript as TimeLimitedSeedPickup;
        }
        else
        {
            regularPickup = pickupScript as RegularItemPickup;
        }
        
        if (promptText != null)
        {
            string itemName = item != null ? item.DisplayName : "Item";
            promptText.text = $"Inventory Full! Select a slot to replace with {itemName}:";
            if (promptText.gameObject != replacementPanel)
                promptText.gameObject.SetActive(true);
        }
        
        RefreshSlotDisplays();
        
        if (replacementPanel != null)
        {
            replacementPanel.SetActive(true);
            
            Canvas canvas = replacementPanel.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 100;
            }
            
            var panelImage = replacementPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.raycastTarget = false;
            }
            
            var allImages = replacementPanel.GetComponentsInChildren<Image>(true);
            foreach (var img in allImages)
            {
                if (img.GetComponent<Button>() != null || img.transform.parent?.GetComponent<Button>() != null)
                {
                    continue;
                }
                
                if (img.gameObject == replacementPanel)
                {
                    continue;
                }
                
                img.raycastTarget = false;
            }
            
            if (promptText != null)
            {
                promptText.raycastTarget = false;
            }
        }
        
        SetupSlotButtons();
    }
    
    private void RefreshSlotDisplays()
    {
        int maxSlots = isSeedReplacement ? SeedInventory.Capacity : RegularInventory.Capacity;
        
        for (int i = 0; i < slotButtons.Length && i < slotIcons.Length; i++)
        {
            bool shouldShow = i < maxSlots;
            
            if (slotButtons[i] != null)
            {
                slotButtons[i].gameObject.SetActive(shouldShow);
                if (shouldShow)
                {
                    slotButtons[i].interactable = true;
                }
            }
            
            if (slotIcons[i] != null)
            {
                slotIcons[i].gameObject.SetActive(shouldShow);
            }
            
            if (slotLabels[i] != null)
            {
                slotLabels[i].gameObject.SetActive(shouldShow);
            }
            
            if (!shouldShow) continue;
            
            ItemBase currentItem = null;
            
            if (isSeedReplacement && seedInventory != null)
            {
                currentItem = seedInventory.GetAt(i);
            }
            else if (!isSeedReplacement && regularInventory != null)
            {
                currentItem = regularInventory.GetAt(i);
            }
            
            if (slotIcons[i] != null)
            {
                slotIcons[i].sprite = currentItem != null ? currentItem.Icon : null;
                slotIcons[i].enabled = currentItem != null;
            }
            
            if (slotLabels[i] != null)
            {
                slotLabels[i].text = currentItem != null ? currentItem.DisplayName : "Empty";
            }
        }
    }
    
    private void OnSlotSelected(int slotIndex)
    {
        if (itemToAdd == null) return;
        
        if (isSeedReplacement)
        {
            if (seedPickup != null)
            {
                seedPickup.ConfirmReplacement(slotIndex);
            }
        }
        else
        {
            if (regularPickup != null)
            {
                regularPickup.ConfirmReplacement(slotIndex);
            }
        }
        
        if (replacementPanel != null)
            replacementPanel.SetActive(false);
        
        if (promptText != null && promptText.gameObject != replacementPanel)
            promptText.gameObject.SetActive(false);
        
        itemToAdd = null;
    }
    
    public void CancelReplacement()
    {
        if (replacementPanel != null)
            replacementPanel.SetActive(false);
        
        if (promptText != null && promptText.gameObject != replacementPanel)
            promptText.gameObject.SetActive(false);
        
        itemToAdd = null;
        seedPickup = null;
        regularPickup = null;
    }
}


