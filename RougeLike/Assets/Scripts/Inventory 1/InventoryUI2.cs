using UnityEngine;
using System.Collections.Generic;

public class InventoryUI2 : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private ItemDescriptionPanel descriptionPanel;
    [SerializeField] private InventorySlotButton[] slotButtons;

    [Header("Settings")]
    [SerializeField] private ItemCategory inventoryType = ItemCategory.Item;

    private MonoBehaviour inventory;

    private void Awake()
    {
        // Find description panel if not assigned
        if (descriptionPanel == null)
        {
            Transform descParent = transform.parent?.Find("Description");
            if (descParent == null)
            {
                // Try searching in scene
                GameObject descObj = GameObject.Find("Description");
                if (descObj != null)
                    descriptionPanel = descObj.GetComponent<ItemDescriptionPanel>();
            }
            else
            {
                descriptionPanel = descParent.GetComponent<ItemDescriptionPanel>();
            }
        }

        // Find slot buttons if not assigned
        if (slotButtons == null || slotButtons.Length == 0)
        {
            List<InventorySlotButton> foundSlots = new List<InventorySlotButton>();
            GetComponentsInChildren<InventorySlotButton>(foundSlots);
            slotButtons = foundSlots.ToArray();
        }

        // Get inventory reference based on type
        switch (inventoryType)
        {
            case ItemCategory.Seed:
                inventory = SeedInventory2.Instance;
                break;
            case ItemCategory.Item:
                inventory = RegularInventory2.Instance;
                break;
            case ItemCategory.Weapon:
                inventory = WeaponInventory2.Instance;
                break;
        }

        // Initialize slot buttons - set their indices and types
        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (slotButtons[i] != null)
            {
                slotButtons[i].Initialize(i, inventoryType);
            }
        }
    }

    private void OnEnable()
    {
        // Re-get inventory reference in case it wasn't ready during Awake
        if (inventory == null)
        {
            switch (inventoryType)
            {
                case ItemCategory.Seed:
                    inventory = SeedInventory2.Instance;
                    break;
                case ItemCategory.Item:
                    inventory = RegularInventory2.Instance;
                    break;
                case ItemCategory.Weapon:
                    inventory = WeaponInventory2.Instance;
                    break;
            }
        }
        
        SubscribeToInventory();
        RefreshUI();
    }
    
    private void Start()
    {
        // Also try to refresh in Start in case OnEnable was called before inventory was ready
        if (inventory == null)
        {
            switch (inventoryType)
            {
                case ItemCategory.Seed:
                    inventory = SeedInventory2.Instance;
                    break;
                case ItemCategory.Item:
                    inventory = RegularInventory2.Instance;
                    break;
                case ItemCategory.Weapon:
                    inventory = WeaponInventory2.Instance;
                    break;
            }
            
            if (inventory != null)
            {
                SubscribeToInventory();
                RefreshUI();
            }
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromInventory();
    }

    private void SubscribeToInventory()
    {
        if (inventory is SeedInventory2 seedInv)
        {
            seedInv.OnChanged += RefreshUI;
            seedInv.OnSelectionChanged += OnSelectionChanged;
        }
        else if (inventory is RegularInventory2 regularInv)
        {
            regularInv.OnChanged += RefreshUI;
            regularInv.OnSelectionChanged += OnSelectionChanged;
        }
        else if (inventory is WeaponInventory2 weaponInv)
        {
            weaponInv.OnChanged += RefreshUI;
            weaponInv.OnSelectionChanged += OnSelectionChanged;
        }
    }

    private void UnsubscribeFromInventory()
    {
        if (inventory is SeedInventory2 seedInv)
        {
            seedInv.OnChanged -= RefreshUI;
            seedInv.OnSelectionChanged -= OnSelectionChanged;
        }
        else if (inventory is RegularInventory2 regularInv)
        {
            regularInv.OnChanged -= RefreshUI;
            regularInv.OnSelectionChanged -= OnSelectionChanged;
        }
        else if (inventory is WeaponInventory2 weaponInv)
        {
            weaponInv.OnChanged -= RefreshUI;
            weaponInv.OnSelectionChanged -= OnSelectionChanged;
        }
    }

    public void OnSlotClicked(int slotIndex)
    {
        if (inventory == null) return;

        ItemType2 item = GetItemAt(slotIndex);
        int currentSelected = GetSelectedIndex();

        if (item != null)
        {
            SetSelectedIndex(slotIndex);
            ShowItemDescription(item);
        }
        else if (item == null && currentSelected == slotIndex)
        {
            // Clear selection if clicking empty slot
            SetSelectedIndex(-1);
            if (descriptionPanel != null)
                descriptionPanel.Clear();
        }
    }

    private ItemType2 GetItemAt(int index)
    {
        if (inventory is SeedInventory2 seedInv) return seedInv.GetAt(index);
        if (inventory is RegularInventory2 regularInv) return regularInv.GetAt(index);
        if (inventory is WeaponInventory2 weaponInv) return weaponInv.GetAt(index);
        return null;
    }

    private int GetSelectedIndex()
    {
        if (inventory is SeedInventory2 seedInv) return seedInv.SelectedIndex;
        if (inventory is RegularInventory2 regularInv) return regularInv.SelectedIndex;
        if (inventory is WeaponInventory2 weaponInv) return weaponInv.SelectedIndex;
        return -1;
    }

    private void SetSelectedIndex(int index)
    {
        if (inventory is SeedInventory2 seedInv) seedInv.SelectedIndex = index;
        else if (inventory is RegularInventory2 regularInv) regularInv.SelectedIndex = index;
        else if (inventory is WeaponInventory2 weaponInv) weaponInv.SelectedIndex = index;
    }

    private void OnSelectionChanged(int selectedIndex)
    {
        // Update visual selection state
        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (slotButtons[i] != null)
            {
                slotButtons[i].SetSelected(i == selectedIndex);
            }
        }

        // Show description for selected item
        int capacity = GetCapacity();
        if (selectedIndex >= 0 && selectedIndex < capacity)
        {
            ItemType2 item = GetItemAt(selectedIndex);
            if (item != null)
            {
                ShowItemDescription(item);
            }
        }
        else
        {
            if (descriptionPanel != null)
                descriptionPanel.Clear();
        }
    }

    private int GetCapacity()
    {
        if (inventory is SeedInventory2) return SeedInventory2.Capacity;
        if (inventory is RegularInventory2) return RegularInventory2.Capacity;
        if (inventory is WeaponInventory2) return WeaponInventory2.Capacity;
        return 0;
    }

    private void ShowItemDescription(ItemType2 item)
    {
        if (descriptionPanel != null && item != null)
        {
            descriptionPanel.ShowItem(item);
        }
    }

    public void RefreshUI()
    {
        if (inventory == null || slotButtons == null || slotButtons.Length == 0) return;

        int capacity = GetCapacity();

        // Update all slot visuals
        for (int i = 0; i < slotButtons.Length && i < capacity; i++)
        {
            if (slotButtons[i] != null)
            {
                ItemType2 item = GetItemAt(i);
                slotButtons[i].UpdateSlot(item);
            }
        }
    }

    public InventorySlotButton[] GetSlotButtons()
    {
        return slotButtons;
    }
}

