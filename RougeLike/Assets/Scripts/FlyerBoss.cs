using UnityEngine;

public class FlyerBoss : MonoBehaviour
{
    public float speed = 3f;
    public float damage = 20f; // Damage dealt to player on collision
    public LayerMask playerLayer; // Set this to the Player layer in the Inspector

    private int direction = 1; // 1 for right, -1 for left
    private Camera mainCamera;
    private float leftBound;
    private float rightBound;
    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        CalculateCameraBounds();

        // Ensure boss doesn't fall initially
        rb.gravityScale = 0;
        
        // Find player and set direction based on relative position
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            float playerX = player.position.x;
            
            // If boss is left of player, move right (1). If boss is right of player, move left (-1).
            direction = transform.position.x < playerX ? 1 : -1;
        }
        
        // Ignore collision with everything except player
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            // Ignore platforms
            GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
            foreach (GameObject platform in platforms)
            {
                Collider2D platformCollider = platform.GetComponent<Collider2D>();
                if (platformCollider != null)
                {
                    Physics2D.IgnoreCollision(myCollider, platformCollider, true);
                }
            }
            
            // Ignore all enemies
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
            foreach (GameObject enemy in enemies)
            {
                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
                if (enemyCollider != null && enemyCollider != myCollider)
                {
                    Physics2D.IgnoreCollision(myCollider, enemyCollider, true);
                }
            }
        }
        
        // Destroy after 5 seconds
        Destroy(gameObject, 5f);
    }

    void FixedUpdate()
    {
        Move();
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
        

        // Apply velocity
        rb.linearVelocity = new Vector2(direction * speed, 0);
    }

    void FlipSprite()
    {
        // Flip the sprite by negating the X scale
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"FlyerBoss collision with: {collision.gameObject.name}, tag: {collision.gameObject.tag}");
        
        // Check if we collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Player_Stats playerStats = collision.gameObject.GetComponent<Player_Stats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
                Debug.Log($"FlyerBoss hit player for {damage} damage!");
            }
            else
            {
                Debug.LogWarning("Player_Stats component not found on player!");
            }
            
            
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"FlyerBoss trigger with: {collision.gameObject.name}, tag: {collision.gameObject.tag}");
        
        // Check if we collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Player_Stats playerStats = collision.gameObject.GetComponent<Player_Stats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
                Debug.Log($"FlyerBoss hit player for {damage} damage!");
            }
            else
            {
                Debug.LogWarning("Player_Stats component not found on player!");
            }
            
            // Destroy the flying boss after hitting the player
            Destroy(gameObject);
        }
    }
}
