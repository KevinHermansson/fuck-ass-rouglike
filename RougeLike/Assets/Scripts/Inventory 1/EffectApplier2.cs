using UnityEngine;
using System.Collections.Generic;

public class EffectApplier2 : MonoBehaviour
{
    [SerializeField] private Player_Stats playerStats;

    private List<ItemType2> lastApplied = new List<ItemType2>();

    private void OnEnable()
    {
        if (SeedInventory2.Instance != null)
            SeedInventory2.Instance.OnChanged += Reapply;
        if (RegularInventory2.Instance != null)
            RegularInventory2.Instance.OnChanged += Reapply;
        if (WeaponInventory2.Instance != null)
            WeaponInventory2.Instance.OnChanged += Reapply;
        
        Reapply();
    }

    private void OnDisable()
    {
        if (SeedInventory2.Instance != null)
            SeedInventory2.Instance.OnChanged -= Reapply;
        if (RegularInventory2.Instance != null)
            RegularInventory2.Instance.OnChanged -= Reapply;
        if (WeaponInventory2.Instance != null)
            WeaponInventory2.Instance.OnChanged -= Reapply;
        
        RemoveAll();
    }

    private void RemoveAll()
    {
        foreach (var item in lastApplied)
        {
            if (item != null && item.Effect2 != null)
            {
                item.Effect2.Remove(playerStats);
            }
        }
        lastApplied.Clear();
    }

    private void Reapply()
    {
        if (playerStats == null) return;

        RemoveAll();

        // Apply effects from all inventories
        ApplyInventoryEffects(SeedInventory2.Instance);
        ApplyInventoryEffects(RegularInventory2.Instance);
        ApplyInventoryEffects(WeaponInventory2.Instance);
    }

    private void ApplyInventoryEffects(MonoBehaviour inventory)
    {
        if (inventory == null) return;

        int capacity = 0;
        if (inventory is SeedInventory2) capacity = SeedInventory2.Capacity;
        else if (inventory is RegularInventory2) capacity = RegularInventory2.Capacity;
        else if (inventory is WeaponInventory2) capacity = WeaponInventory2.Capacity;

        for (int i = 0; i < capacity; i++)
        {
            ItemType2 item = null;
            if (inventory is SeedInventory2) item = SeedInventory2.Instance.GetAt(i);
            else if (inventory is RegularInventory2) item = RegularInventory2.Instance.GetAt(i);
            else if (inventory is WeaponInventory2) item = WeaponInventory2.Instance.GetAt(i);

            if (item != null && item.Effect2 != null)
            {
                item.Effect2.Apply(playerStats);
                lastApplied.Add(item);
            }
        }
    }
}

