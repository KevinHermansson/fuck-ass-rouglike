using UnityEngine;

public class SeedEffectApplier : MonoBehaviour
{
    [SerializeField] private SeedInventoryHolder holder;
    [SerializeField] private PlayerStats stats;

    private SeedItem[] lastApplied = new SeedItem[SeedInventory.Capacity];

    private void OnEnable()
    {
        holder.Inventory.OnChanged += Reapply;
        Reapply();
    }

    private void OnDisable()
    {
        holder.Inventory.OnChanged -= Reapply;
        RemoveAll();
    }

    private void RemoveAll()
    {
        for (int i = 0; i < lastApplied.Length; i++)
        {
            if (lastApplied[i] != null && lastApplied[i].Effect != null)
                lastApplied[i].Effect.Remove(stats);
            lastApplied[i] = null;
        }
    }

    private void Reapply()
    {
        RemoveAll();

        for (int i = 0; i < SeedInventory.Capacity; i++)
        {
            var item = holder.Inventory.GetAt(i);
            if (item != null && item.Effect != null)
            {
                item.Effect.Apply(stats);
                lastApplied[i] = item;
            }
        }
    }
}
