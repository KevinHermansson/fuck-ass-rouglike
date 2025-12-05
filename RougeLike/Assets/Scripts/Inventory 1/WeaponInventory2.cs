using UnityEngine;
using System;

public class WeaponInventory2 : MonoBehaviour
{
    public const int Capacity = 2;
    public static WeaponInventory2 Instance { get; private set; }

    [SerializeField] private ItemType2[] slots = new ItemType2[Capacity];
    [SerializeField] private int selectedIndex = -1;

    public event Action OnChanged;
    public event Action<int> OnSelectionChanged;

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            if (selectedIndex != value)
            {
                selectedIndex = value;
                OnSelectionChanged?.Invoke(selectedIndex);
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (slots == null || slots.Length != Capacity)
        {
            slots = new ItemType2[Capacity];
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public ItemType2 GetAt(int index)
    {
        return (index >= 0 && index < Capacity) ? slots[index] : null;
    }

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

    public bool AddItem(ItemType2 item)
    {
        if (item == null || item.Category != ItemCategory.Weapon) return false;

        if (HasEmptySlot(out int idx))
        {
            slots[idx] = item;
            OnChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= Capacity) return;
        slots[index] = null;
        OnChanged?.Invoke();
    }

    public void ReplaceAt(int index, ItemType2 newItem)
    {
        if (index < 0 || index >= Capacity) return;
        if (newItem == null || newItem.Category != ItemCategory.Weapon) return;
        slots[index] = newItem;
        OnChanged?.Invoke();
    }

    public void Clear()
    {
        for (int i = 0; i < Capacity; i++)
        {
            slots[i] = null;
        }
        OnChanged?.Invoke();
    }
}

