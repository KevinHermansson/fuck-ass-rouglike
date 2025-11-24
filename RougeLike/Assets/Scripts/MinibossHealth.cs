using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MinibossHealth : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 1000;
    public Image Healthbarfordummies; // Drag the UI Image (boss health bar) here in Inspector
    public TextMeshProUGUI healthText; // Drag the TextMeshPro text inside the health bar here
    public GameObject healthBarBorder; // Drag the GameObject for the health bar border here
    public float flashDuration = 0.1f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // Transparent red

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
        if (Healthbarfordummies != null)
        {
            Healthbarfordummies.gameObject.SetActive(false);
        }
        if (healthText != null)
        {
            healthText.gameObject.SetActive(false);
        }
        if (healthBarBorder != null)
        {
            healthBarBorder.gameObject.SetActive(false);
        }
        // Add death animation, rewards, etc.
        Destroy(gameObject, 1f);
    }
}
