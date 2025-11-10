using UnityEngine;

public class Mushroom_movement : MonoBehaviour
{
    public float speed = 5f;
    public float awakeDistance = 5f;
    public float stopDistance = 8f;
    public float jumpForce = 5f;
    public float jumpInterval = 3f;
    public float knockbackForce = 10f;
    public float attackCooldown = 1f;
    public Rigidbody2D rb;
    public Animator animator;
    public float damage;

    Transform player;
    private bool isChasing = false;
    private float jumpTimer = 0f;
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
        HandleJump();
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
        Debug.Log("OnAttackComplete called!");

        if (player == null)
        {
            Debug.LogWarning("Player is null!");
            return;
        }

        float distance = Vector2.Distance(rb.position, (Vector2)player.position);
        Debug.Log($"Distance to player: {distance}");

        // Only apply knockback if player is still in range
        if (distance <= 2f)
        {
            MovementScript playerMovement = player.GetComponent<MovementScript>();
            Player_Health playerHealth = player.GetComponent<Player_Health>();

            if (playerMovement != null)
            {
                Vector2 knockbackDirection = ((Vector2)player.position - rb.position).normalized;
                Vector2 knockbackVelocity = new Vector2(knockbackDirection.x * knockbackForce, 0);

                Debug.Log($"Applying knockback: {knockbackVelocity}, Direction: {knockbackDirection}, Force: {knockbackForce}");
                playerMovement.ApplyKnockback(knockbackVelocity);
            }
            else
            {
                Debug.LogWarning("Player has no MovementScript!");
            }

            // Apply damage to player
            if (playerHealth != null)
            {
                damage = 5;
                playerHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("Player has no Player_Health component!");
            }
        }
        else
        {
            Debug.Log("Player too far for knockback!");
        }

        // Reset attack cooldown
        attackTimer = attackCooldown;
    }

    private void HandleJump()
    {
        if (isChasing && rb.linearVelocity.x != 0)
        {
            jumpTimer += Time.deltaTime;

            if (jumpTimer >= jumpInterval)
            {

                jumpTimer = 0f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
        else
        {
            jumpTimer = 0f;
        }
    }

    private void HandleAnimation(Rigidbody2D rb, Transform player)
    {
        bool isMoving = rb.linearVelocity.x != 0 && !animator.GetBool("isAttack");

        animator.SetBool("isMoving", isMoving);

        bool isAttack = Vector2.Distance(rb.position, (Vector2)player.position) <= 2f;
        animator.SetBool("isAttack", isAttack);
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