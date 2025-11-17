using UnityEngine;

public class InventoryAutoReset : MonoBehaviour
{
    [SerializeField] private SeedInventoryHolder seedHolder;
    [SerializeField] private RegularInventoryHolder regularHolder;

    public void ResetInventory()
    {
        if (seedHolder != null && seedHolder.Inventory != null)
        {
            seedHolder.Inventory.Clear();
            Debug.Log("Seed inventory cleared from button!");
        }

        if (regularHolder != null && regularHolder.Inventory != null)
        {
            regularHolder.Inventory.Clear();
            Debug.Log("Regular inventory cleared from button!");
        }
    }
}
