using UnityEngine;

public class BossSpawnMovement : MonoBehaviour
{
    public float speed = 3f;
    public float fallGravityScale = 1f; // Gravity scale when falling after attack
    public float attackTriggerWidth = 0f; // How close the player needs to be on the X-axis to trigger the attack
    public float splashDamageRadius = 2f; // The radius of the splash damage on impact
    public float splashDamage = 25f; // The damage dealt by the splash attack
    public LayerMask playerLayer; // Set this to the Player layer in the Inspector

    private int direction = 1; // 1 for right, -1 for left
    private Camera mainCamera;
    private float leftBound;
    private float rightBound;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isAttacking = false; // This flag now means "is in the attack-and-fall sequence"
    private Transform player;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        CalculateCameraBounds();

        // Ensure boss doesn't fall initially
        rb.gravityScale = 0;
        
        // Find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;

            // Ignore collision with the player
            Collider2D myCollider = GetComponent<Collider2D>();
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (myCollider != null && playerCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, playerCollider, true);
            }
        }
    }

    void Update()
    {
        // Can't attack if already attacking or if the player doesn't exist
        if (isAttacking || player == null) return;

        // Check if the boss is right over the player (same X level)
        float horizontalDistance = Mathf.Abs(transform.position.x - player.position.x);
        bool isPlayerBelow = transform.position.y > player.position.y;

        // If player is directly underneath (same X level within trigger width) and below the boss, attack
        if (horizontalDistance <= attackTriggerWidth && isPlayerBelow)
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        // Only move normally if not in the attack/fall sequence
        if (!isAttacking)
        {
            Move();
        }
        else
        {
            // When falling, ensure X velocity stays at 0
            if (rb.gravityScale > 0)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    void CalculateCameraBounds()
    {
        if (mainCamera != null)
        {
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            leftBound = mainCamera.transform.position.x - cameraWidth / 2f + 1f;
            rightBound = mainCamera.transform.position.x + cameraWidth / 2f - 1f;
        }
    }

    void Move()
    {
        // Check bounds and reverse direction
        if (transform.position.x >= rightBound && direction == 1)
        {
            direction = -1;
            FlipSprite();
        }
        else if (transform.position.x <= leftBound && direction == -1)
        {
            direction = 1;
            FlipSprite();
        }

        // Apply velocity
        rb.linearVelocity = new Vector2(direction * speed, 0);
    }

    void FlipSprite()
    {
        // Flip the sprite by negating the X scale
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        
        // Stop all movement
        rb.linearVelocity = Vector2.zero;

        // Play the attack animation
        animator.SetBool("IsAttack", true);
    }

    // This should be called by an animation event at the end of the attack animation
    public void OnAttackAnimationFinished()
    {
        // Stop the animation from looping
        animator.SetBool("IsAttack", false);

        // Enable gravity to make the enemy fall
        rb.gravityScale = fallGravityScale;
        
        // Ensure the velocity is reset to allow gravity to take effect
        rb.linearVelocity = new Vector2(0, 0);
        
        Debug.Log("Boss started falling! Gravity scale: " + rb.gravityScale);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we are in the attack/fall sequence and if the object we hit is tagged as "Ground"
        if (isAttacking && collision.gameObject.CompareTag("Ground"))
        {
            // --- Splash Damage Logic ---
            Collider2D[] playersToDamage = Physics2D.OverlapCircleAll(transform.position, splashDamageRadius, playerLayer);
            foreach (Collider2D playerCollider in playersToDamage)
            {
                Player_Stats playerStats = playerCollider.GetComponent<Player_Stats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(splashDamage);
                    Debug.Log($"Boss splashed player for {splashDamage} damage!");
                }
            }

            // --- Conclude Attack Sequence ---
            // Stop all movement and disable gravity
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            
            // Freeze the animator on the last attack frame
            if (animator != null)
            {
                animator.speed = 0;
            }
            
            // Wait a moment before disappearing
            Destroy(gameObject, 0.3f);
        }
    }

    // This is a helper method to visualize the splash damage radius in the Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashDamageRadius);
    }
}
