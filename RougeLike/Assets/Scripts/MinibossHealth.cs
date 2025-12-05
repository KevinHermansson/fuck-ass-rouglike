using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq; // Add this if not present
using TMPro;

public class MinibossHealth : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 1000;
    public Image Healthbarfordummies; // Drag the UI Image (boss health bar) here in Inspector
    public TextMeshProUGUI healthText; // Drag the TextMeshPro text inside the health bar here
    public GameObject healthBarBorder; // Drag the GameObject for the health bar border here
    public GameObject minibossPebblePrefab; // Assign the pebble prefab this miniboss should drop
    public float flashDuration = 0.1f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // Transparent red

    [Header("Item Drop Settings")]
    [SerializeField] private GameObject itemPickupPrefab; // Prefab for item pickup
    [SerializeField] private float itemDropChance = 0.5f; // 50% chance to drop an item
    [SerializeField] private bool dropWeapon = true; // If true, drops weapon; if false, drops seed

    private SpriteRenderer spriteRenderer;
    private bool isFlashing = false;

    void Start()
    {
        health = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (Healthbarfordummies == null)
        {
            Debug.LogError("Healthbarfordummies Image is not assigned in Inspector!");
        }
        else
        {
            Healthbarfordummies.fillAmount = 1f; // Full health at start
        }

        UpdateHealthText();
        
        // Hide health bar initially
        HideHealthBar();
    }
    
    public void ShowHealthBar()
    {
        if (Healthbarfordummies != null)
        {
            Healthbarfordummies.transform.parent.gameObject.SetActive(true);
        }
        if (healthBarBorder != null)
        {
            healthBarBorder.SetActive(true);
        }
    }
    
    public void HideHealthBar()
    {
        if (Healthbarfordummies != null)
        {
            Healthbarfordummies.transform.parent.gameObject.SetActive(false);
        }
        if (healthBarBorder != null)
        {
            healthBarBorder.SetActive(false);
        }
    }

    void Update()
    {
        // Debug: Press I to test damage
        if (Input.GetKeyDown(KeyCode.I))
        {
            TakeDamage(10);
        }

        // Press R to reset color if stuck red
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
                Debug.Log("Reset miniboss color to white");
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        health = Mathf.Clamp(health, 0, maxHealth); // Clamp only when taking damage
        
        float fillValue = health / maxHealth;
        Debug.Log($"Miniboss took {damageAmount} damage. Health: {health}, fillAmount: {fillValue}");

        if (Healthbarfordummies != null)
        {
            Healthbarfordummies.fillAmount = fillValue;
        }

        UpdateHealthText();

        // Trigger damage flash only if not already flashing
        if (spriteRenderer != null && !isFlashing)
        {
            StartCoroutine(DamageFlash());
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            // Show current health / max health (e.g., "980 / 1000")
            healthText.text = health.ToString("F0") + " / " + maxHealth.ToString("F0");
            Debug.Log($"Updated health text to: {healthText.text} (health = {health})");
        }
        else
        {
            Debug.LogWarning("healthText is not assigned in Inspector! Please drag the TextMeshPro component into the Health Text field.");
        }
    }

    IEnumerator DamageFlash()
    {
        if (spriteRenderer == null || isFlashing) yield break;
        
        isFlashing = true;
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        
        // Make sure sprite renderer still exists before resetting color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        isFlashing = false;
    }

    void Die()
    {
        Debug.Log("Miniboss defeated!");
        // Hide health bar
        HideHealthBar();
        
        // Drop pebbles
        if (PebbleManager.Instance != null)
        {
            if (minibossPebblePrefab != null)
            {
                PebbleManager.Instance.DropPebbles(minibossPebblePrefab, 15, transform.position);
            }
            else
            {
                Debug.LogWarning($"Miniboss has no minibossPebblePrefab assigned in MinibossHealth script!");
            }
        }

        // Drop item with chance
        if (Random.value <= itemDropChance)
        {
            DropItem();
        }

        // Add death animation, rewards, etc.
        Destroy(gameObject, 1f);
    }

    private void DropItem()
    {
        ItemType2 itemToDrop = null;

        if (dropWeapon)
        {
            // Drop a random weapon
            ItemType2[] allWeapons = UnityEngine.Resources.FindObjectsOfTypeAll<ItemType2>()
                .Where(item => item.Category == ItemCategory.Weapon)
                .ToArray();

            if (allWeapons.Length > 0)
            {
                itemToDrop = allWeapons[Random.Range(0, allWeapons.Length)];
            }
        }
        else
        {
            // Drop a random seed
            ItemType2[] allSeeds = UnityEngine.Resources.FindObjectsOfTypeAll<ItemType2>()
                .Where(item => item.Category == ItemCategory.Seed)
                .ToArray();

            if (allSeeds.Length > 0)
            {
                itemToDrop = allSeeds[Random.Range(0, allSeeds.Length)];
            }
        }

        if (itemToDrop == null)
        {
            Debug.LogWarning("No items found to drop!");
            return;
        }

        // Create pickup GameObject
        GameObject pickupObj;
        if (itemPickupPrefab != null)
        {
            pickupObj = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            pickupObj = new GameObject($"Pickup_{itemToDrop.DisplayName}");
            pickupObj.transform.position = transform.position;
            
            if (itemToDrop.Icon != null)
            {
                SpriteRenderer sr = pickupObj.AddComponent<SpriteRenderer>();
                sr.sprite = itemToDrop.Icon;
                sr.sortingOrder = 10;
            }
        }

        ItemPickup2 pickup = pickupObj.GetComponent<ItemPickup2>();
        if (pickup == null)
        {
            pickup = pickupObj.AddComponent<ItemPickup2>();
        }

        pickup.SetItem(itemToDrop);
        Debug.Log($"Miniboss dropped: {itemToDrop.DisplayName}");
    }
}
