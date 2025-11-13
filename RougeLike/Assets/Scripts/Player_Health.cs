using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player_Health : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public Image healthBar;
    public float flashDuration = 0.1f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // Transparent red

    private SpriteRenderer spriteRenderer;


    void Start()
    {
        health = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (healthBar == null)
        {
            Debug.LogError("healthBar Image is not assigned in Inspector!");
        }
        else
        {
            Debug.Log($"healthBar assigned. Type: {healthBar.type}");
            healthBar.fillAmount = 1f; // full at start
        }
    }

    void Update()
    {
        // clamp health between 0 and maxHealth
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        float fillValue = health / maxHealth;
        Debug.Log($"Player took {damageAmount} damage. Health: {health}, fillAmount: {fillValue}");

        if (healthBar != null)
        {
            healthBar.fillAmount = fillValue;
        }

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
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Add death logic here (respawn, game over, etc.)
    }
}
