using UnityEngine;

public class InventorySelectionManager : MonoBehaviour
{
    private static InventorySelectionManager instance;
    public static InventorySelectionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<InventorySelectionManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("InventorySelectionManager");
                    instance = go.AddComponent<InventorySelectionManager>();
                }
            }
            return instance;
        }
    }
    
    private SeedInventoryUI seedUI;
    private RegularInventoryUI regularUI;
    private InventoryType currentSelectedType = InventoryType.None;
    
    private enum InventoryType
    {
        None,
        Seed,
        Regular
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public void RegisterSeedUI(SeedInventoryUI ui)
    {
        seedUI = ui;
    }
    
    public void RegisterRegularUI(RegularInventoryUI ui)
    {
        regularUI = ui;
    }
    
    public void SelectSeed(int index)
    {
        if (regularUI != null && regularUI.Inventory != null)
        {
            DeselectRegular();
        }
        
        if (seedUI != null && seedUI.Inventory != null)
        {
            seedUI.Inventory.SelectedIndex = index;
            currentSelectedType = InventoryType.Seed;
        }
    }
    
    public void SelectRegular(int index)
    {
        if (seedUI != null && seedUI.Inventory != null)
        {
            DeselectSeed();
        }
        
        if (regularUI != null && regularUI.Inventory != null)
        {
            regularUI.Inventory.SelectedIndex = index;
            currentSelectedType = InventoryType.Regular;
        }
    }
    
    private void DeselectSeed()
    {
        if (seedUI != null && seedUI.Inventory != null)
        {
            int firstNonEmpty = 0;
            for (int i = 0; i < SeedInventory.Capacity; i++)
            {
                if (seedUI.Inventory.GetAt(i) != null)
                {
                    firstNonEmpty = i;
                    break;
                }
            }
            seedUI.Inventory.SelectedIndex = firstNonEmpty;
        }
    }
    
    private void DeselectRegular()
    {
        if (regularUI != null && regularUI.Inventory != null)
        {
            int firstNonEmpty = 0;
            for (int i = 0; i < RegularInventory.Capacity; i++)
            {
                if (regularUI.Inventory.GetAt(i) != null)
                {
                    firstNonEmpty = i;
                    break;
                }
            }
            regularUI.Inventory.SelectedIndex = firstNonEmpty;
        }
    }
    
    public bool IsSeedSelected()
    {
        return currentSelectedType == InventoryType.Seed;
    }
    
    public bool IsRegularSelected()
    {
        return currentSelectedType == InventoryType.Regular;
    }
}
