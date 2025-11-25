using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player_Stats : MonoBehaviour
{
    public float health = 100;
    public Image healthBar;
    public float flashDuration = 0.1f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f);

    // Base Stats
    [Header("Base Stats")]
    public float BaseMoveSpeed = 5f;
    public int BaseMaxHealth = 100;
    public int BaseAttackDamage = 20;
    public float BaseAttackSpeed = 1f;
    public float BaseJumpHeight = 8f;

    // Current Stats (read-only in inspector)
    [Header("Current Stats (Base + Bonuses)")]
    [SerializeField] private float currentMoveSpeed;
    [SerializeField] private int currentMaxHealth;
    [SerializeField] private int currentAttackDamage;
    [SerializeField] private float currentAttackSpeed;
    [SerializeField] private float currentJumpHeight;

    public float SpeedBonus { get; set; }
    public int MaxHealthBonus { get; set; }
    public int AttackDamageBonus { get; set; }
    public float AttackSpeedBonus { get; set; }
    public float JumpHeightBonus { get; set; }

    public float MoveSpeed => BaseMoveSpeed + SpeedBonus;
    public int MaxHealth => BaseMaxHealth + MaxHealthBonus;
    public int AttackDamage => BaseAttackDamage + AttackDamageBonus;
    public float AttackSpeed => BaseAttackSpeed + AttackSpeedBonus;
    public float JumpHeight => BaseJumpHeight + JumpHeightBonus;

    private SpriteRenderer spriteRenderer;
    private MovementScript movementScript;
    private bool isFlashing = false;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<MovementScript>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

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

        // Update current stats display in inspector
        currentMoveSpeed = MoveSpeed;
        currentMaxHealth = MaxHealth;
        currentAttackDamage = AttackDamage;
        currentAttackSpeed = AttackSpeed;
        currentJumpHeight = JumpHeight;
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

    IEnumerator DamageFlash()
    {
        isFlashing = true;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }

    void Die()
    {
        Debug.Log("Player died!");
    }
}
