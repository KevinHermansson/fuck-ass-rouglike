using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Inventory/SeedInventory")]
public class SeedInventory : ScriptableObject
{
    public const int Capacity = 6;
    [SerializeField] private SeedItem[] slots = new SeedItem[Capacity];
    [SerializeField] private int selectedIndex = 0;

    public event Action OnChanged;
    
    private void OnEnable()
    {
        if (slots == null || slots.Length != Capacity)
        {
            slots = new SeedItem[Capacity];
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

    public SeedItem GetAt(int index) => (index >= 0 && index < Capacity) ? slots[index] : null;

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

    public void AddOrReplace(SeedItem newSeed, out SeedItem replaced)
    {
        if (newSeed == null) { replaced = null; return; }

        if (HasEmptySlot(out int idx))
        {
            slots[idx] = newSeed;
            replaced = null;
        }
        else
        {
            replaced = slots[selectedIndex];
            slots[selectedIndex] = newSeed;
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
