using UnityEngine;

public class SeedPickup : MonoBehaviour
{
    public SeedItem seedToGive;
    public GameObject seedPickupPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var holder = other.GetComponentInParent<SeedInventoryHolder>();
        if (holder == null || holder.Inventory == null || seedToGive == null) return;

        holder.Inventory.AddOrReplace(seedToGive, out SeedItem replaced);

        if (replaced != null && seedPickupPrefab != null)
        {
            var go = Instantiate(seedPickupPrefab, transform.position, Quaternion.identity);
            var pickup = go.GetComponent<SeedPickup>();
            if (pickup != null) pickup.seedToGive = replaced;
        }

        Destroy(gameObject);
    }
}
