using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player_Stats : MonoBehaviour
{
    public float health = 100;
    public Image healthBar;
    public float flashDuration = 0.1f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f);

    // Stats with bonuses
    public float BaseMoveSpeed = 5f;
    public int BaseMaxHealth = 100;

    public float SpeedBonus { get; set; }
    public int MaxHealthBonus { get; set; }

    public float MoveSpeed => BaseMoveSpeed + SpeedBonus;
    public int MaxHealth => BaseMaxHealth + MaxHealthBonus;

    private SpriteRenderer spriteRenderer;
    private MovementScript movementScript;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<MovementScript>();

        health = MaxHealth;

        if (healthBar == null)
        {
            Debug.LogError("healthBar Image is not assigned in Inspector!");
        }
        else
        {
            healthBar.fillAmount = 1f;
        }
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, MaxHealth);

        // Apply speed bonus to movement script
        if (movementScript != null)
        {
            movementScript.moveSpeed = MoveSpeed;
        }
    }

    public void ClampHealth()
    {
        if (health > MaxHealth) health = MaxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        float fillValue = health / MaxHealth;
        Debug.Log($"Player took {damageAmount} damage. Health: {health}/{MaxHealth}, fillAmount: {fillValue}");

        if (healthBar != null)
        {
            healthBar.fillAmount = fillValue;
        }

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
    }
}
