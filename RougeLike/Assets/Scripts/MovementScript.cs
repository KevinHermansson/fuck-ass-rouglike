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

    public float groundCheck = 0;
    public float wallCheck = 0;
    public bool fancyGroundCheck;
    BoxCollider2D PlayerCollider;

    // CHANGED: remove isKnockedBack gating, use additive knockback
    private float knockbackTimer = 0f;
    private Vector2 knockbackAdd = Vector2.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

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

        // jump once when pressed
        if (Input.GetKeyDown(KeyCode.Space) && groundCheck >= 1)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);

        if (Input.GetKeyDown(KeyCode.Space) && wallCheck >= 1)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);

        if (Input.GetKeyDown(KeyCode.Space) && fancyGroundCheck)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);


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

    // set groundCheck when colliding with objects tagged "Ground"
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            groundCheck = 1;
        if (collision.collider.CompareTag("fancyPlatform"))
            fancyGroundCheck = true;
        if (collision.collider.CompareTag("Wall"))
            wallCheck += 1;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            groundCheck -= 1;

        if (collision.collider.CompareTag("Wall"))
            wallCheck -= 1;

        if (collision.collider.CompareTag("fancyPlatform"))
            fancyGroundCheck = false;
    }

    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        PlayerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        PlayerCollider.enabled = true;
    }

    public void dropThroughPlatform()
    {
        if (Input.GetKey(KeyCode.S) && fancyGroundCheck && PlayerCollider.enabled)
            StartCoroutine(DisablePlayerCollider(0.40f));
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
