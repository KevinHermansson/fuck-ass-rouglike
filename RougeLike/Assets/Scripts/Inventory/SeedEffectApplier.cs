using UnityEngine;

public class SeedEffectApplier : MonoBehaviour
{
    [SerializeField] private SeedInventoryHolder holder;
    [SerializeField] private PlayerStats stats;

    private SeedItem[] lastApplied = new SeedItem[SeedInventory.Capacity];

    private void OnEnable()
    {
        if (holder == null)
        {
            Debug.LogError("[SeedEffectApplier] SeedInventoryHolder is not assigned!");
            return;
        }

        if (holder.Inventory == null)
        {
            Debug.LogError("[SeedEffectApplier] SeedInventory is not assigned in holder!");
            return;
        }

        if (playerStats == null)
        {
            Debug.LogError("[SeedEffectApplier] Player_Stats is not assigned!");
            return;
        }

        holder.Inventory.OnChanged += Reapply;
        Reapply();
    }

    private void OnDisable()
    {
        if (holder != null && holder.Inventory != null)
        {
            holder.Inventory.OnChanged -= Reapply;
        }
        RemoveAll();
    }

    private void RemoveAll()
    {
        if (playerStats == null) return;

        for (int i = 0; i < lastApplied.Length; i++)
        {
            if (lastApplied[i] != null && lastApplied[i].Effect != null)
<<<<<<< Updated upstream
                lastApplied[i].Effect.Remove(stats);
=======
            {
                lastApplied[i].Effect.Remove(playerStats);
            }
>>>>>>> Stashed changes
            lastApplied[i] = null;
        }
    }

    private void Reapply()
    {
        if (holder == null || holder.Inventory == null || playerStats == null)
        {
            Debug.LogWarning("[SeedEffectApplier] Cannot reapply effects - missing required components.");
            return;
        }

        RemoveAll();

        for (int i = 0; i < SeedInventory.Capacity; i++)
        {
            var item = holder.Inventory.GetAt(i);
            if (item != null && item.Effect != null)
            {
                item.Effect.Apply(stats);
                lastApplied[i] = item;
                Debug.Log($"[SeedEffectApplier] Applied effect '{item.Effect.EffectName}' from seed '{item.name}' at slot {i}");
            }
        }
    }
}
