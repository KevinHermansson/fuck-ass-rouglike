using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private KeyCode dropKey = KeyCode.Q;
    [SerializeField] private float dropOffset = 1f; // Distance in front of player
    [SerializeField] private GameObject itemPickupPrefab; // Prefab with ItemPickup2 component

    private Transform player;
    private PlayerAttack playerAttack;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerAttack = playerObj.GetComponent<PlayerAttack>();
        }
    }

    private float GetPlayerFacingDirection()
    {
        // Try to get direction from PlayerAttack first (most reliable)
        if (playerAttack != null)
        {
            var lastDirectionField = typeof(PlayerAttack).GetField("lastDirection", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (lastDirectionField != null)
            {
                float lastDirection = (float)lastDirectionField.GetValue(playerAttack);
                return lastDirection;
            }
        }

        // Fallback: Check sprite renderer
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite != null)
        {
            return playerSprite.flipX ? -1f : 1f;
        }

        // Fallback: Check current input
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            return -1f; // Facing left
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            return 1f; // Facing right
        }

        // Default to right
        return 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(dropKey))
        {
            TryDropSelectedItem();
        }
    }

    private void TryDropSelectedItem()
    {
        if (player == null) return;

        // Check seed inventory first
        if (SeedInventory2.Instance != null && SeedInventory2.Instance.SelectedIndex >= 0)
        {
            ItemType2 item = SeedInventory2.Instance.GetAt(SeedInventory2.Instance.SelectedIndex);
            if (item != null)
            {
                // Seeds get deleted
                SeedInventory2.Instance.RemoveAt(SeedInventory2.Instance.SelectedIndex);
                SeedInventory2.Instance.SelectedIndex = -1;
                Debug.Log($"Deleted seed: {item.DisplayName}");
                return;
            }
        }

        // Check regular inventory
        if (RegularInventory2.Instance != null && RegularInventory2.Instance.SelectedIndex >= 0)
        {
            ItemType2 item = RegularInventory2.Instance.GetAt(RegularInventory2.Instance.SelectedIndex);
            if (item != null)
            {
                DropItem(item, RegularInventory2.Instance, RegularInventory2.Instance.SelectedIndex);
                return;
            }
        }

        // Check weapon inventory
        if (WeaponInventory2.Instance != null && WeaponInventory2.Instance.SelectedIndex >= 0)
        {
            ItemType2 item = WeaponInventory2.Instance.GetAt(WeaponInventory2.Instance.SelectedIndex);
            if (item != null)
            {
                DropItem(item, WeaponInventory2.Instance, WeaponInventory2.Instance.SelectedIndex);
                return;
            }
        }
    }

    private void DropItem(ItemType2 item, MonoBehaviour inventory, int slotIndex)
    {
        if (item == null || player == null) return;

        // Get player's facing direction
        float direction = GetPlayerFacingDirection();

        Vector3 dropPosition = player.position + Vector3.right * dropOffset * direction;

        // Create pickup GameObject
        GameObject pickupObj = CreateItemPickup(item, dropPosition);

        // Remove from inventory
        if (inventory is RegularInventory2)
        {
            RegularInventory2.Instance.RemoveAt(slotIndex);
            RegularInventory2.Instance.SelectedIndex = -1;
        }
        else if (inventory is WeaponInventory2)
        {
            WeaponInventory2.Instance.RemoveAt(slotIndex);
            WeaponInventory2.Instance.SelectedIndex = -1;
        }

        Debug.Log($"Dropped: {item.DisplayName}");
    }

    private GameObject CreateItemPickup(ItemType2 item, Vector3 position)
    {
        GameObject pickupObj;

        // Use prefab if assigned, otherwise create new GameObject
        if (itemPickupPrefab != null)
        {
            pickupObj = Instantiate(itemPickupPrefab, position, Quaternion.identity);
        }
        else
        {
            pickupObj = new GameObject($"Pickup_{item.DisplayName}");
            pickupObj.transform.position = position;
            
            // Add sprite renderer if item has icon
            if (item.Icon != null)
            {
                SpriteRenderer sr = pickupObj.AddComponent<SpriteRenderer>();
                sr.sprite = item.Icon;
                sr.sortingOrder = 10;
            }
        }

        // Add ItemPickup2 component
        ItemPickup2 pickup = pickupObj.GetComponent<ItemPickup2>();
        if (pickup == null)
        {
            pickup = pickupObj.AddComponent<ItemPickup2>();
        }

        // Set the item using reflection or a public method
        // Since ItemPickup2 has itemToGive as private, we'll need to set it via reflection
        // Or better: add a public method to ItemPickup2 to set the item
        SetItemOnPickup(pickup, item);

        return pickupObj;
    }

    private void SetItemOnPickup(ItemPickup2 pickup, ItemType2 item)
    {
        pickup.SetItem(item);
    }

    public void DropItemAtSlot(ItemType2 item, ItemCategory category, int slotIndex)
    {
        if (item == null || player == null) return;

        // Get player's facing direction
        float direction = GetPlayerFacingDirection();
        Vector3 dropPosition = player.position + Vector3.right * dropOffset * direction;

        // Create pickup GameObject
        CreateItemPickup(item, dropPosition);

        // Remove from inventory
        if (category == ItemCategory.Item && RegularInventory2.Instance != null)
        {
            RegularInventory2.Instance.RemoveAt(slotIndex);
        }
        else if (category == ItemCategory.Weapon && WeaponInventory2.Instance != null)
        {
            WeaponInventory2.Instance.RemoveAt(slotIndex);
        }

        Debug.Log($"Dropped: {item.DisplayName}");
    }
}

