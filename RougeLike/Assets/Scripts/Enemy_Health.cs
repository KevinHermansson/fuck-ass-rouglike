using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;

public class Enemy_Health : MonoBehaviour
{
    public float health = 20;
    public float maxHealth = 20;
    public int pebblesToDrop = 1;
    public GameObject enemyPebblePrefab;
    public TextMeshProUGUI healthText;
    public float textOffsetX = 0f;
    public float textOffsetY = 0f;
    public float flashDuration = 0.1f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f);

    [Header("Special Enemy Settings")]
    [SerializeField] private bool canBeSpecial = false;
    [SerializeField] private float specialHealthMultiplier = 1.5f;
    [SerializeField] private float specialSpawnChance = 0.25f;
    [SerializeField] private GameObject itemPickupPrefab;

    private bool isSpecial = false;
    private float baseMaxHealth;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private Canvas canvas;

    void Start()
    {
        baseMaxHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (canBeSpecial && Random.value <= specialSpawnChance)
        {
            isSpecial = true;
            maxHealth = baseMaxHealth * specialHealthMultiplier;
            health = maxHealth;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1f, 0.8f, 0f, 1f);
                originalColor = spriteRenderer.color;
            }
        }
        else
        {
            health = maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = health.ToString("F0");
            healthText.alignment = TextAlignmentOptions.Center;
            
            canvas = healthText.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }
        }
    }

    void Update()
    {
        // Debug: Press I to take 1 damage
        if (Input.GetKeyDown(KeyCode.I))
        {
            TakeDamage(1);
        }

        // clamp health between 0 and maxHealth
        health = Mathf.Clamp(health, 0, maxHealth);

        // Update health text display
        if (healthText != null)
        {
            healthText.text = health.ToString("F0"); // Display health as whole number
            
            // Position canvas directly above enemy every frame (world position)
            if (canvas != null)
            {
                // Set world position to be above enemy with X and Y offsets
                canvas.transform.position = new Vector3(transform.position.x + textOffsetX, transform.position.y + textOffsetY, transform.position.z);
                
                // Reset rotation to no rotation
                canvas.transform.rotation = Quaternion.identity;
            }
            
            // Make text face the camera
            if (Camera.main != null)
            {
                healthText.transform.rotation = Camera.main.transform.rotation;
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        // Trigger damage flash
        if (spriteRenderer != null)
        {
            StartCoroutine(DamageFlash());
        }

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        if (spriteRenderer == null) yield break;
        
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died!");

        if (PebbleManager.Instance != null && pebblesToDrop > 0)
        {
            if (enemyPebblePrefab != null)
            {
                PebbleManager.Instance.DropPebbles(enemyPebblePrefab, pebblesToDrop, transform.position);
            }
            else
            {
                Debug.LogWarning($"Enemy '{gameObject.name}' has no enemyPebblePrefab assigned in Enemy_Health script!");
            }
        }

        if (isSpecial)
        {
            DropRandomSeed();
        }

        Destroy(gameObject);
    }

    private void DropRandomSeed()
    {
        ItemType2[] allSeeds = UnityEngine.Resources.FindObjectsOfTypeAll<ItemType2>()
            .Where(item => item.Category == ItemCategory.Seed)
            .ToArray();

        if (allSeeds.Length == 0)
        {
            Debug.LogWarning("No seed items found to drop!");
            return;
        }

        ItemType2 randomSeed = allSeeds[Random.Range(0, allSeeds.Length)];

        GameObject pickupObj;
        if (itemPickupPrefab != null)
        {
            pickupObj = Instantiate(itemPickupPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            pickupObj = new GameObject($"Pickup_{randomSeed.DisplayName}");
            pickupObj.transform.position = transform.position;
            
            if (randomSeed.Icon != null)
            {
                SpriteRenderer sr = pickupObj.AddComponent<SpriteRenderer>();
                sr.sprite = randomSeed.Icon;
                sr.sortingOrder = 10;
            }
        }

        ItemPickup2 pickup = pickupObj.GetComponent<ItemPickup2>();
        if (pickup == null)
        {
            pickup = pickupObj.AddComponent<ItemPickup2>();
        }

        pickup.SetItem(randomSeed);
        Debug.Log($"Special enemy dropped seed: {randomSeed.DisplayName}");
    }
}
