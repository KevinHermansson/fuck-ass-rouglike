using UnityEngine;

public class Mushroom_movement : MonoBehaviour
{
    public float speed = 5f;
    public float awakeDistance = 5f;
    public float stopDistance = 8f;
    public Rigidbody2D rb;

    Transform player;
    private bool isChasing = false;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;

        // Get the Animator component
        animator = GetComponent<Animator>();

        // Set initial velocity to 0
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    // FixedUpdate is called at fixed intervals for physics
    void FixedUpdate()
    {
        if (player == null || rb == null)
            return;

        // Calculate distance between mushroom and player
        float distance = Vector2.Distance(rb.position, (Vector2)player.position);

        // Start chasing if player is within awake distance
        if (!isChasing && distance <= awakeDistance)
        {
            isChasing = true;
            
            // Trigger animation when starting to chase
            if (animator != null)
                animator.SetBool("isMoving", true);
        }
        
        // Stop chasing if player is beyond stop distance
        if (isChasing && distance > stopDistance)
        {
            isChasing = false;
            
            // Stop animation when stopping chase
            if (animator != null)
                animator.SetBool("isMoving", false);
        }

        // If chasing, move towards player
        if (isChasing)
        {
            // Calculate direction to player (only X axis)
            float directionX = ((Vector2)player.position - rb.position).normalized.x;
            
            // Set only X velocity, keep Y velocity unchanged
            rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);
            
            // Rotate based on movement direction
            if (directionX < 0)
            {
                // Moving left - rotate to 0, 180, 0
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (directionX > 0)
            {
                // Moving right - rotate to 0, 0, 0
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            // Stop horizontal movement, keep Y velocity unchanged
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}