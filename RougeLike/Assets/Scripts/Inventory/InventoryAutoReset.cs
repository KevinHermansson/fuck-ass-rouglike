using UnityEngine;

public class InventoryAutoReset : MonoBehaviour
{
    [SerializeField] private SeedInventoryHolder holder;

    public void ResetInventory()
    {
        if (holder != null && holder.Inventory != null)
        {
            holder.Inventory.Clear();
            Debug.Log("Inventory cleared from button!");
        }
    }
}
