using UnityEngine;

public class Popcorn_shot : MonoBehaviour
{
    public float shotSpeed = 5f;
    public int damage = 5;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 direction;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Find player
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            // Calculate direction towards player (only X axis)
            float directionX = player.position.x - transform.position.x;
            direction = directionX > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            // If no player, shoot right by default
            direction = Vector2.right;
        }

        // Setup Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Configure Rigidbody2D
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        rb.linearVelocity = direction * shotSpeed;

        // Setup Collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
        }

        // Ignore collisions with all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null && collider != null)
            {
                Physics2D.IgnoreCollision(collider, enemyCollider);
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

    void FixedUpdate()
    {
        // Keep popcorn moving at constant speed (only in X direction)
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction.x * shotSpeed, 0);
        }

        // Check if popcorn left the screen
        if (IsOffScreen())
        {
            Destroy(gameObject);
        }
    }

    bool IsOffScreen()
    {
        if (mainCamera == null) return false;

        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        
        // Check if outside screen bounds (with small buffer)
        return screenPoint.x < -0.1f || screenPoint.x > 1.1f || 
               screenPoint.y < -0.1f || screenPoint.y > 1.1f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;

        // Check if popcorn hits the ground or walls
        if (tag == "Ground" || tag == "fancyPlatform" || tag == "Wall")
        {
            Destroy(gameObject);
            return;
        }

        // Check if popcorn hits the player
        if (tag == "Player")
        {
            ApplyDamage(collision.gameObject);
            Destroy(gameObject);
            return;
        }
    }

    private void ApplyDamage(GameObject playerObject)
    {
        Player_Health playerHealth = playerObject.GetComponent<Player_Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log($"Popcorn hit player for {damage} damage!");
        }
    }
}
