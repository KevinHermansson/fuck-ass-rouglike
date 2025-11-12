using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Inventory/RegularInventory")]
public class RegularInventory : ScriptableObject
{
    [SerializeField] private ItemBase[] slots;
    public event Action OnChanged;

    public void Init(int capacity)
    {
        slots = new ItemBase[Mathf.Max(1, capacity)];
        OnChanged?.Invoke();
    }

    public int Capacity => slots?.Length ?? 0;
    public ItemBase GetAt(int index) => (index >= 0 && index < Capacity) ? slots[index] : null;

    public bool TryAdd(ItemBase item)
    {
        if (item == null || item.Category != ItemCategory.Regular) return false;
        for (int i = 0; i < Capacity; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                OnChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Capacity) return;
        slots[index] = null;
        OnChanged?.Invoke();
    }
}
