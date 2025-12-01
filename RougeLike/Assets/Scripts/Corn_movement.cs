using UnityEngine;

public class Corn_movement : MonoBehaviour
{
    public float speed = 2f;
    public float fleeDistance = 5f; // Distance at which corn starts running away
    public float approachDistance = 6f; // Distance at which corn approaches player
    public GameObject popcornPrefab; // Assign the popcorn prefab in the Inspector
    public float spawnInterval = 1f; // Time between popcorn spawns
    public float edgeBuffer = 0.5f; // Distance from camera edge
    public Rigidbody2D rb;
    public float damage;

    Transform player;
    private bool isFleeing = false;
    private bool isApproaching = false;
    private float spawnTimer = 0f;
    private Camera mainCamera;
    private float leftBound;
    private float rightBound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            UpdateCameraBounds();
        }

        // Ignore collisions with all other enemies and player
        Collider2D collider = GetComponent<Collider2D>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy != gameObject) // Don't ignore collision with self
            {
                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
                if (enemyCollider != null && collider != null)
                {
                    Physics2D.IgnoreCollision(collider, enemyCollider);
                }
            }
        }

        // Ignore collision with player
        if (p != null)
        {
            Collider2D playerCollider = p.GetComponent<Collider2D>();
            if (playerCollider != null && collider != null)
            {
                Physics2D.IgnoreCollision(collider, playerCollider);
            }
        }

        // Ignore collisions with all bullets
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
            if (bulletCollider != null && collider != null)
            {
                Physics2D.IgnoreCollision(collider, bulletCollider);
            }
        }
    }

    private void UpdateCameraBounds()
    {
        if (mainCamera == null) return;

        float cornY = transform.position.y;
        float camDistance = Mathf.Abs(mainCamera.transform.position.z - cornY);

        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, camDistance));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, camDistance));

        leftBound = leftEdge.x + edgeBuffer;
        rightBound = rightEdge.x - edgeBuffer;
    }

    void Update()
    {
        
    }

   

    void FixedUpdate()
    {
        if (player == null || rb == null)
            return;

        float distance = Vector2.Distance(rb.position, (Vector2)player.position);
        
        // Always rotate towards player
        float playerDirectionX = player.position.x - transform.position.x;
        if (playerDirectionX < 0)
        {
            // Player is to the left - rotate 180 (face left)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // Player is to the right - no rotation (face right)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        
        // Check if player is on same Y level (within threshold)
        float yDifference = Mathf.Abs(player.position.y - transform.position.y);
        bool onSameLevel = yDifference < 1f; // 1f is threshold for movement
        bool canShoot = yDifference < 1f; // 2f is threshold for shooting

        // Handle popcorn spawning (only if Y difference is less than 2f)
        if (canShoot)
        {
            spawnTimer += Time.fixedDeltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnPopcorn();
                spawnTimer = 0f;
            }
        }
        else
        {
            // Reset timer when player is too far away vertically
            spawnTimer = 0f;
        }

        // Determine behavior based on distance
        if (distance <= fleeDistance)
        {
            // Too close - flee
            isFleeing = true;
            isApproaching = false;
        }
        else if (distance > approachDistance && onSameLevel)
        {
            // Too far AND on same level - approach
            isFleeing = false;
            isApproaching = true;
        }
        else
        {
            // In the sweet spot or not on same level - stop
            isFleeing = false;
            isApproaching = false;
        }

        // Update camera bounds
        UpdateCameraBounds();

        // Check camera boundaries
        bool atLeftEdge = rb.position.x <= leftBound;
        bool atRightEdge = rb.position.x >= rightBound;

        // If fleeing, move away from player
        if (isFleeing)
        {
            // Calculate direction away from player
            float directionX = ((Vector2)player.position - rb.position).normalized.x;
            // Invert direction to move away
            directionX = -directionX;

            // Don't move if at edge and trying to move further out
            if ((atLeftEdge && directionX < 0) || (atRightEdge && directionX > 0))
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);
            }
        }
        // If too far, approach player
        else if (isApproaching)
        {
            // Calculate direction towards player
            float directionX = ((Vector2)player.position - rb.position).normalized.x;

            // Don't move if at edge and trying to move further out
            if ((atLeftEdge && directionX < 0) || (atRightEdge && directionX > 0))
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);
            }
        }
        else
        {
            // Stop moving when in sweet spot
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void SpawnPopcorn()
    {
        if (popcornPrefab == null)
        {
            Debug.LogWarning("Popcorn prefab is not assigned!");
            return;
        }

        // Calculate spawn position in front of corn based on rotation
        float spawnOffsetX = 0.5f; // Distance in front of corn
        float spawnOffsetY = 0.3f; // Height above corn
        Vector3 spawnOffset;
        
        // Check which way corn is facing based on rotation
        if (transform.rotation.eulerAngles.y == 0)
        {
            // Facing right
            spawnOffset = new Vector3(spawnOffsetX, spawnOffsetY, 0);
        }
        else
        {
            // Facing left (rotation is 180)
            spawnOffset = new Vector3(-spawnOffsetX, spawnOffsetY, 0);
        }
        
        Vector3 spawnPosition = transform.position + spawnOffset;
        GameObject popcorn = Instantiate(popcornPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Popcorn spawned!");
    }
}
