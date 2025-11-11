using UnityEngine;

public class Slime_movement : MonoBehaviour
{
    public float speed = 3f;
    public float awakeDistance = 7f;
    public float stopDistance = 10f;
    public float knockbackForce = 4f;
    public float attackCooldown = 3f;
    public Rigidbody2D rb;
    public Animator animator;

    public float damage = 5f;

    Transform player;
    private bool isChasing = false;
    private float attackTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        HandleAnimation(rb, player);
        HandleAttack();
    }

    private void HandleAttack()
    {
        if (player == null) return;

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    // This method is called by the Animation Event at the end of the attack animation
    public void OnAttackComplete()
    {
        Debug.Log("Slime OnAttackComplete called!");

        if (player == null)
        {
            Debug.LogWarning("Player is null!");
            return;
        }

        float distance = Vector2.Distance(rb.position, (Vector2)player.position);
        Debug.Log($"Distance to player: {distance}");

        // Check if player is above the slime
        float yDifference = player.position.y - transform.position.y;
        bool playerIsAbove = yDifference > 0.5f; // 0.5 is threshold

        // Only apply knockback and damage if player is still in range AND not above
        if (distance <= 2f && !playerIsAbove)
        {
            MovementScript playerMovement = player.GetComponent<MovementScript>();

            if (playerMovement != null)
            {
                Vector2 knockbackDirection = ((Vector2)player.position - rb.position).normalized;
                Vector2 knockbackVelocity = new Vector2(knockbackDirection.x * knockbackForce, 0f);

                Debug.Log($"Applying knockback: {knockbackVelocity}, Direction: {knockbackDirection}, Force: {knockbackForce}");
                playerMovement.ApplyKnockback(knockbackVelocity);
            }

            TakeDamage();
        }
        else if (playerIsAbove)
        {
            Debug.Log("Player is above slime - no damage dealt!");
        }
        else
        {
            Debug.Log("Player too far for knockback!");
        }

        // Reset attack cooldown
        attackTimer = attackCooldown;
    }



    private void HandleAnimation(Rigidbody2D rb, Transform player)
    {
        bool isMoving = rb.linearVelocity.x != 0 && !animator.GetBool("isAttack");
        animator.SetBool("isMoving", isMoving);

        // Check if player is above
        float yDifference = player.position.y - transform.position.y;
        bool playerIsAbove = yDifference > 0.5f;

        // Only trigger attack if player is in range AND not above
        bool isAttack = Vector2.Distance(rb.position, (Vector2)player.position) <= 2f && !playerIsAbove;
        animator.SetBool("isAttack", isAttack);
    }

    private void TakeDamage()
    {
        Player_Health playerHealth = player.GetComponent<Player_Health>();
        if (playerHealth != null)
        {
            damage = 5;
            playerHealth.TakeDamage(damage);
        }
    }

    void FixedUpdate()
    {
        float distance = Vector2.Distance(rb.position, (Vector2)player.position);
        if (player == null || rb == null)
            return;

        // Start chasing if player is within awake distance
        if (!isChasing && distance <= awakeDistance)
        {
            isChasing = true;

            if (animator != null)
                animator.SetBool("isMoving", true);
        }

        // Stop chasing if player is beyond stop distance
        if (isChasing && distance > stopDistance)
        {
            isChasing = false;

            if (animator != null)
                animator.SetBool("isMoving", false);
        }

        // Stop moving at 1f distance
        bool shouldStopMoving = distance <= 1f;

        // If chasing, move towards player (but stop at 1f distance)
        if (isChasing && !shouldStopMoving)
        {
            float directionX = ((Vector2)player.position - rb.position).normalized.x;

            rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);

            // Rotate based on movement direction
            if (directionX < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (directionX > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}
