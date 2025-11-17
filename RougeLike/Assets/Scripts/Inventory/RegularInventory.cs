using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Inventory/RegularInventory")]
public class RegularInventory : ScriptableObject
{
    public const int Capacity = 6;
    [SerializeField] private ItemBase[] slots = new ItemBase[Capacity];
    [SerializeField] private int selectedIndex = 0;

    public event Action OnChanged;

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            selectedIndex = Mathf.Clamp(value, 0, Capacity - 1);
            OnChanged?.Invoke();
        }
    }

    public ItemBase GetAt(int index) => (index >= 0 && index < Capacity) ? slots[index] : null;

    public bool HasEmptySlot(out int emptyIndex)
    {
        for (int i = 0; i < Capacity; i++)
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
        if (newItem == null || newItem.Category != ItemCategory.Regular) { replaced = null; return; }

        if (HasEmptySlot(out int idx))
        {
            slots[idx] = newItem;
            replaced = null;
        }
        else
        {
            replaced = slots[selectedIndex];
            slots[selectedIndex] = newItem;
        }

        OnChanged?.Invoke();
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Capacity) return;
        slots[index] = null;
        OnChanged?.Invoke();
    }

    public void Clear()
    {
        for (int i = 0; i < Capacity; i++) slots[i] = null;
        selectedIndex = 0;
        OnChanged?.Invoke();
    }
}
