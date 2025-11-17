using UnityEngine;

public class RegularItemPickup : MonoBehaviour
{
    public RegularItem itemToGive;
    public GameObject itemPickupPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var holder = other.GetComponentInParent<RegularInventoryHolder>();
        if (holder == null || holder.Inventory == null || itemToGive == null) return;

        holder.Inventory.AddOrReplace(itemToGive, out ItemBase replaced);

        if (replaced != null && itemPickupPrefab != null)
        {
            var go = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
            var pickup = go.GetComponent<RegularItemPickup>();
            if (pickup != null) pickup.itemToGive = replaced as RegularItem;
        }

        Destroy(gameObject);
    }
}

