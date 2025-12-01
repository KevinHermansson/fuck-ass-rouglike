using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MovementScript : MonoBehaviour
{
    public float jumpHeight = 8;
    public float moveSpeed = 5;
    public float knockbackDuration = 0.3f;
    public Rigidbody2D rb;
    public Animator animator;
    private Player_Stats playerStats;

    public float groundCheck = 0;
    public float wallCheck = 0;
    public bool fancyGroundCheck;
    public BoxCollider2D feetTrigger; // Assign the feet trigger collider in Inspector
    BoxCollider2D PlayerCollider;

    private bool feetTouchingGround = false;

    // CHANGED: remove isKnockedBack gating, use additive knockback
    private float knockbackTimer = 0f;
    private Vector2 knockbackAdd = Vector2.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<Player_Stats>();

        // Ignore collisions with all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null && PlayerCollider != null)
            {
                Physics2D.IgnoreCollision(PlayerCollider, enemyCollider);
            }
        }
    }

    void Update()
    {
        // Check every frame if feet collider is touching ground or platform
        CheckFeetCollider();

        // decay knockback
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            float t = (knockbackTimer / knockbackDuration);
            // smooth decay
            knockbackAdd *= t;
            if (knockbackTimer <= 0f)
                knockbackAdd = Vector2.zero;
        }

        // Use jump height from playerStats if available, otherwise use default
        float currentJumpHeight = (playerStats != null) ? playerStats.JumpHeight : jumpHeight;

        // jump once when pressed
        if (Input.GetKey(KeyCode.Space) && groundCheck >= 1)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpHeight);

        if (Input.GetKey(KeyCode.Space) && wallCheck >= 1)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpHeight);

        if (Input.GetKey(KeyCode.Space) && fancyGroundCheck)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJumpHeight);


        if (Input.GetKey(KeyCode.S))
            dropThroughPlatform();

        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);

        float inputVX = 0f;
        if (left && !right)
        {
            inputVX = -moveSpeed;

            // Set animator parameters for moving left
            if (animator != null)
            {
                animator.SetBool("MovingLeft", true);
                animator.SetBool("MovingRight", false);
            }
        }
        else if (right && !left)
        {
            inputVX = moveSpeed;

            // Set animator parameters for moving right
            if (animator != null)
            {
                animator.SetBool("MovingLeft", false);
                animator.SetBool("MovingRight", true);
            }
        }
        else
        {
            // Player is not moving (standing still)
            if (animator != null)
            {
                animator.SetBool("MovingLeft", false);
                animator.SetBool("MovingRight", false);
            }
        }

        // combine input with knockback additive x
        float finalVX = inputVX + knockbackAdd.x;
        rb.linearVelocity = new Vector2(finalVX, rb.linearVelocity.y + knockbackAdd.y);

        if (Input.GetKey(KeyCode.S))
            dropThroughPlatform();
    }

    void CheckFeetCollider()
    {
        if (feetTrigger == null) return;

        // Check if feet collider is overlapping with Ground
        Collider2D[] groundColliders = Physics2D.OverlapBoxAll(
            feetTrigger.bounds.center,
            feetTrigger.bounds.size,
            0f
        );

        bool touchingGround = false;
        bool touchingPlatform = false;

        foreach (Collider2D col in groundColliders)
        {
            if (col == feetTrigger) continue; // Skip self

            if (col.CompareTag("Ground"))
            {
                touchingGround = true;
            }

            if (col.CompareTag("fancyPlatform"))
            {
                touchingPlatform = true;
            }
        }

        // Update groundCheck based on what feet are touching
        if (touchingGround || touchingPlatform)
        {
            groundCheck = 1;
        }
        else
        {
            groundCheck = 0;
        }

        // Update fancyGroundCheck
        fancyGroundCheck = touchingPlatform;
    }

    // Trigger detection for feet collider (used for ground detection)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if this trigger is the feet trigger
        if (feetTrigger != null && other != feetTrigger)
        {
            if (other.CompareTag("Ground"))
            {
                feetTouchingGround = true;
                groundCheck = 1;
            }

            if (other.CompareTag("fancyPlatform"))
            {
                fancyGroundCheck = true;
                groundCheck = 1;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if this trigger is the feet trigger
        if (feetTrigger != null && other != feetTrigger)
        {
            if (other.CompareTag("Ground"))
            {
                feetTouchingGround = false;
                groundCheck = 0;
            }

            if (other.CompareTag("fancyPlatform"))
            {
                fancyGroundCheck = false;
                groundCheck = 0;
            }
        }
    }

    // set wallCheck when colliding with objects tagged "Wall"
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
            wallCheck += 1;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
            wallCheck -= 1;
    }

    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        // Temporarily disable both the feet trigger AND main collider to allow dropping through
        if (feetTrigger != null)
            feetTrigger.enabled = false;

        if (PlayerCollider != null)
            PlayerCollider.enabled = false;

        yield return new WaitForSeconds(disableTime);

        if (feetTrigger != null)
            feetTrigger.enabled = true;

        if (PlayerCollider != null)
            PlayerCollider.enabled = true;
    }

    public void dropThroughPlatform()
    {
        bool canDrop = (feetTrigger != null ? feetTrigger.enabled : true) && (PlayerCollider != null ? PlayerCollider.enabled : true);
        if (Input.GetKey(KeyCode.S) && fancyGroundCheck && canDrop)
            StartCoroutine(DisablePlayerCollider(0.30f));
    }

    // UPDATED: additive knockback
    public void ApplyKnockback(Vector2 knockbackVelocity)
    {
        // add instead of override
        knockbackAdd += knockbackVelocity;
        knockbackTimer = knockbackDuration;
        Debug.Log($"Knockback added: {knockbackVelocity}, total additive: {knockbackAdd}");
    }
}
