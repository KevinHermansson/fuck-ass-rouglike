using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Inventory/RegularInventory")]
public class RegularInventory : ScriptableObject
{
    public const int Capacity = 9;
    [SerializeField] private ItemBase[] slots = new ItemBase[Capacity];
    [SerializeField] private int selectedIndex = 0;

    public event Action OnChanged;
    
    private void OnEnable()
    {
        if (slots == null || slots.Length != Capacity)
        {
            slots = new ItemBase[Capacity];
        }
    }

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            selectedIndex = Mathf.Clamp(value, 0, Capacity - 1);
            OnChanged?.Invoke();
        }
    }

    public ItemBase GetAt(int index)
    {
        if (slots == null || slots.Length != Capacity)
        {
            slots = new ItemBase[Capacity];
        }
        
        return (index >= 0 && index < Capacity && index < slots.Length) ? slots[index] : null;
    }

    public bool HasEmptySlot(out int emptyIndex)
    {
        if (slots == null || slots.Length != Capacity)
        {
            slots = new ItemBase[Capacity];
        }
        
        for (int i = 0; i < Capacity && i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                emptyIndex = i;
                return true;
            }
        }
        emptyIndex = -1;
        return false;
    }

    public void AddOrReplace(ItemBase newItem, out ItemBase replaced)
    {
        if (slots == null || slots.Length != Capacity)
        {
            slots = new ItemBase[Capacity];
        }
        
        if (newItem == null || newItem.Category != ItemCategory.Regular) { replaced = null; return; }

        if (HasEmptySlot(out int idx))
        {
            if (idx >= 0 && idx < slots.Length)
            {
                slots[idx] = newItem;
                replaced = null;
            }
            else
            {
                replaced = null;
                return;
            }
        }
        else
        {
            if (selectedIndex >= 0 && selectedIndex < slots.Length)
            {
                replaced = slots[selectedIndex];
                slots[selectedIndex] = newItem;
            }
            else
            {
                replaced = null;
                return;
            }
        }

        OnChanged?.Invoke();
    }

    public void RemoveAt(int index)
    {
        if (slots == null || slots.Length != Capacity)
        {
            slots = new ItemBase[Capacity];
        }
        
        if (index < 0 || index >= Capacity || index >= slots.Length) return;
        slots[index] = null;
        OnChanged?.Invoke();
    }

    public void Clear()
    {
        if (slots == null || slots.Length != Capacity)
        {
            slots = new ItemBase[Capacity];
        }
        
        for (int i = 0; i < Capacity && i < slots.Length; i++) slots[i] = null;
        selectedIndex = 0;
        OnChanged?.Invoke();
    }
}
