using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player_Stats : MonoBehaviour
{
    public float health = 100;
    public Image healthBar;
    public GameObject gameOverUI; // Drag the Game Over UI GameObject here
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
    private static Player_Stats instance;
    private bool isDead = false;

    void Awake()
    {
        // Singleton pattern - keep only one player instance across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void OnDestroy()
    {
        // Reset singleton when destroyed
        if (instance == this)
        {
            instance = null;
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<MovementScript>();


        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            // Make sure sprite is visible when scene loads
            spriteRenderer.enabled = true;
        }

        // Reset death state and health when scene loads
        isDead = false;
        health = MaxHealth;
        
        // Re-enable movement script
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
        
        // Reset rigidbody to dynamic
        Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.bodyType = RigidbodyType2D.Dynamic;
        }

        if (healthBar == null)
        {
            Debug.LogError("healthBar Image is not assigned in Inspector!");
        }
        else
        {
            healthBar.fillAmount = health / MaxHealth;
        }

        // Always hide Game Over UI when entering any scene
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
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
        if (isDead) return; // Prevent multiple calls
        isDead = true;

        Debug.Log("Player died!");

        // Freeze player movement
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // Stop player's rigidbody
        Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
            playerRb.bodyType = RigidbodyType2D.Static;
        }

        // Hide player sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Freeze all enemies
        FreezeAllEnemies();
        
        // Show Game Over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        
        // Don't freeze time - it prevents buttons from working
        // Time.timeScale = 0f;
    }

    void FreezeAllEnemies()
    {
        // Find and freeze all enemy movement scripts
        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (MonoBehaviour script in allScripts)
        {
            // Freeze common enemy movement scripts
            if (script is Corn_movement ||
                script is FlyingBat_movement ||
                script is Miniboss_Movement ||
                script is FlyerBoss ||
                script.GetType().Name.Contains("movement") ||
                script.GetType().Name.Contains("Movement") ||
                script.GetType().Name.Contains("Behavior"))
            {
                script.enabled = false;
            }
        }

        // Also freeze all Rigidbody2D components on enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.linearVelocity = Vector2.zero;
                enemyRb.angularVelocity = 0f;
                enemyRb.bodyType = RigidbodyType2D.Static;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet") && other.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            TakeDamage(10);
        }
    }
}
