using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Enemy_Health : MonoBehaviour
{
    public float health = 20;
    public float maxHealth = 20;
    public TextMeshProUGUI healthText; // Assign the TextMeshPro text in Inspector
    public float textOffsetX = 0f; // How far left/right from center (negative = left)
    public float textOffsetY = 0f; // How far above the enemy the text appears
    public float flashDuration = 0.1f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // Transparent red

    private SpriteRenderer spriteRenderer;
    private Canvas canvas;

    void Start()
    {
        health = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Update health text if assigned
        if (healthText != null)
        {
            healthText.text = health.ToString("F0");
            
            // Center the text alignment
            healthText.alignment = TextAlignmentOptions.Center;
            
            // Set up Canvas if it's a World Space Canvas
            canvas = healthText.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                // Set appropriate scale for world space
                canvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }
        }
    }

    void Update()
    {
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
        Destroy(gameObject);
    }
}
