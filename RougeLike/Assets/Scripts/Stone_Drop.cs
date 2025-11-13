using UnityEngine;

public class Stone_Drop : MonoBehaviour
{
    public float dropSpeed = 5f; 
    public int damage = 10;


    private Rigidbody2D rb; 
    Transform player;

    
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;
        
       
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Säkerställ att Rigidbody2D är korrekt konfigurerad
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX; // Lås rotation och X-position
        rb.linearVelocity = Vector2.down * dropSpeed;

        
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
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
        // Keep stone falling at constant speed (won't be affected by collisions)
        if (rb != null)
        {
            rb.linearVelocity = Vector2.down * dropSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        

        // Kolla om stenen träffar marken (flera möjliga taggar)
        string tag = collision.gameObject.tag;
        
        // Check if stone hits the ground
        if (tag == "Ground" || tag == "fancyPlatform")
        {
            Destroy(gameObject); 
            return;
        }
        
        // Check if stone hits the player
        if (tag == "Player")
        {
            TakeDamage();
            Destroy(gameObject); 
            return;
        }
    }
    
    private void TakeDamage()
    {
        if (player == null)
        {
            // Try to find player again
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
                player = p.transform;
            else
                return; // Still no player found, exit
        }
        
        Player_Health playerHealth = player.GetComponent<Player_Health>();
        if (playerHealth != null)
        {
            damage = 15;
            playerHealth.TakeDamage(damage);
        }
    }
}
