using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MovementScript : MonoBehaviour
{
    public float jumpHeight = 8;
    public float moveSpeed = 5;
    public float knockbackDuration = 0.3f;
    public Rigidbody2D rb;


    public float groundCheck = 0;
    public float wallCheck = 0;
    public bool fancyGroundCheck;
    BoxCollider2D PlayerCollider;

    private float knockbackTimer = 0f;
    private bool isKnockedBack = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle knockback timer
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
            }
            return; // Skip input during knockback
        }

        // jump once when pressed
        if (Input.GetKeyDown(KeyCode.Space) && groundCheck >= 1)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        }

        if (Input.GetKeyDown(KeyCode.Space) && wallCheck >= 1)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        }
        if (Input.GetKeyDown(KeyCode.Space) && fancyGroundCheck == true)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        }
        if (Input.GetKey(KeyCode.S))
        {
            dropThroughPlatform();
        }

        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);

        float vx = 0f;
        if (left && !right)
        {
            vx = -moveSpeed;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (right && !left)
        {
            vx = moveSpeed;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // both pressed (or neither) -> stop horizontal movement
            vx = 0f;
        }


        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);

        if (Input.GetKey(KeyCode.S))
        {
            dropThroughPlatform();
        }
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
        {
            StartCoroutine(DisablePlayerCollider(0.5f));
        }
    }

    // Public method to apply knockback from external scripts
    public void ApplyKnockback(Vector2 knockbackVelocity)
    {
        rb.linearVelocity = knockbackVelocity;
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        Debug.Log($"Knockback applied! Velocity set to: {knockbackVelocity}");
    }


}
