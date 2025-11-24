using UnityEngine;
using System.Collections;

public class Miniboss_Movement : MonoBehaviour
{
    public Animator animator;
    public float speed = 5f;
    public float leftBound = -8f;
    public float rightBound = 8f;
    public float attackInterval = 4f; // How often to attack
    public float teleportInterval = 8f; // How often to teleport when enraged
    public float attackAnimationDuration = 1f; // How long the attack animation takes
    
    // Attack 1 settings
    public Transform attack1Point; // Position where attack1 damage is checked
    public float attack1Range = 2f; // How far attack1 reaches
    public float attack1Damage = 20f; // Damage dealt by attack1
    
    // Attack 2 settings
    public Transform attack2Point; // Position where attack2 damage is checked
    public float attack2Range = 3f; // How far attack2 reaches
    public float attack2Damage = 30f; // Damage dealt by attack2
    
    public GameObject minibossBallPrefab; // Assign the Minibossball prefab here
    public float minibossBallSpawnInterval = 10f; // Interval for spawning Minibossball
    private float minibossBallSpawnTimer;

    public LayerMask playerLayer; // Layer mask for player
    
    private float attackTimer = 0f;
    private float teleportTimer = 0f;
    private bool isAttacking = false;
    private bool isTeleporting = false;
    private Transform player;
    private int direction = 1; // 1 for right, -1 for left
    private Camera mainCamera;
    private MinibossHealth healthScript;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Start()
    {
        animator = GetComponent<Animator>();
        healthScript = GetComponent<MinibossHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        attackTimer = attackInterval; // Start attack timer
        teleportTimer = teleportInterval; // Initialize teleport timer
        minibossBallSpawnTimer = minibossBallSpawnInterval; // Initialize miniboss ball spawn timer
        mainCamera = Camera.main;
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (isAttacking || healthScript == null) return; // Don't do anything while attacking or if health script is missing

        // Handle all timers
        attackTimer -= Time.deltaTime;
        minibossBallSpawnTimer -= Time.deltaTime;

        // --- Health-Based Logic ---
        bool isEnraged = healthScript.health <= healthScript.maxHealth / 2;

        if (isEnraged)
        {
            teleportTimer -= Time.deltaTime;
            if (teleportTimer <= 0f)
            {
                Teleport();
                teleportTimer = teleportInterval; // Reset timer
                return; // Prioritize teleporting over other actions this frame
            }
        }

        // --- Action Logic ---

        // Regular Attack
        if (attackTimer <= 0f)
        {
            StartCoroutine(AttackSequence());
            attackTimer = attackInterval; // Reset timer
        }
        // Spawn Minibossball (less frequent action, so check with else if)
        else if (minibossBallSpawnTimer <= 0f && minibossBallPrefab != null)
        {
            SpawnMinibossBall();
            minibossBallSpawnTimer = minibossBallSpawnInterval; // Reset timer
        }
        // Move only if not performing another action and not in the middle of a teleport
        else if (!isTeleporting)
        {
            Move();
        }
    }

    void Move()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (transform.position.x >= rightBound)
        {
            direction = -1;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (transform.position.x <= leftBound)
        {
            direction = 1;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    

    

    IEnumerator AttackSequence()
    {
        isAttacking = true;

        // Randomly choose between Attack1 and Attack2
        int randomAttack = Random.Range(1, 3); // Returns 1 or 2

        if (randomAttack == 1)
        {
            yield return StartCoroutine(Attack1Sequence());
        }
        else
        {
            yield return StartCoroutine(Attack2Sequence());
        }

        isAttacking = false;
    }

    IEnumerator Attack1Sequence()
    {
        // Play Attack1 animation
        if (animator != null)
        {
            animator.SetBool("Attack1", true);
        }

        // Wait for animation to finish
        yield return new WaitForSeconds(attackAnimationDuration);

        // Set Attack1 to false
        if (animator != null)
        {
            animator.SetBool("Attack1", false);
        }
    }

    IEnumerator Attack2Sequence()
    {
        // Play Attack2 animation
        if (animator != null)
        {
            animator.SetBool("Attack2", true);
        }

        // Wait for animation to finish
        yield return new WaitForSeconds(attackAnimationDuration);

        // Set Attack2 to false
        if (animator != null)
        {
            animator.SetBool("Attack2", false);
        }
    }

    public void Attack1Hit()
    {
        DealDamageToPlayer(attack1Point, attack1Range, attack1Damage);
    }

    public void Attack2Hit()
    {
        DealDamageToPlayer(attack2Point, attack2Range, attack2Damage);
    }

    void DealDamageToPlayer(Transform attackPoint, float range, float damage)
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("Attack Point not assigned on Miniboss!");
            return;
        }

        // Check for player in attack range
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, range, playerLayer);
        
        foreach (Collider2D player in hitPlayers)
        {
            Player_Health playerHealth = player.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Miniboss dealt {damage} damage to player!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw attack1 range in red
        if (attack1Point != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attack1Point.position, attack1Range);
        }
        
        // Draw attack2 range in yellow
        if (attack2Point != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attack2Point.position, attack2Range);
        }
    }

    void SpawnMinibossBall()
    {
        if (minibossBallPrefab != null)
        {
            // Spawn the ball above the miniboss
            Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0);
            GameObject ball = Instantiate(minibossBallPrefab, spawnPosition, Quaternion.identity);
            
            // Ignore collision between the ball and the miniboss
            Collider2D ballCollider = ball.GetComponent<Collider2D>();
            Collider2D minibossCollider = GetComponent<Collider2D>();
            if (ballCollider != null && minibossCollider != null)
            {
                Physics2D.IgnoreCollision(ballCollider, minibossCollider);
            }
            
            Debug.Log("Minibossball spawned!");
        }
        else
        {
            Debug.LogWarning("Minibossball Prefab is not assigned!");
        }
    }

    // Called by the Update loop timer
    void Teleport()
    {
        if (animator != null)
        {
            isTeleporting = true;
            animator.SetBool("Teleport", true);
        }
    }

    // This function should be called by an Animation Event during the 'Teleport' animation
    public void ChangePositionAndSpawn()
    {
        // Move to a new random position
        float randomX = Random.Range(leftBound, rightBound);
        transform.position = new Vector3(randomX, transform.position.y, transform.position.z);

        if (animator != null)
        {
            // End the teleport animation and start the spawn animation
            animator.SetBool("Teleport", false);
            animator.SetBool("Spawn", true);
        }
    }

    // This function should be called by an Animation Event at the end of the 'Spawn' animation
    public void FinishTeleport()
    {
        if (animator != null)
        {
            animator.SetBool("Spawn", false);
        }
        isTeleporting = false;
    }
}

