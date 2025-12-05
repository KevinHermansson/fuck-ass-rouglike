using UnityEngine;

public class FlyerBoss : MonoBehaviour
{
    public float speed = 3f;
    public float damage = 20f;

    private int direction = 1;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Check Rigidbody2D
        if (rb == null)
        {
            Debug.LogError("FlyerBoss: No Rigidbody2D found! Adding one...");
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Ensure boss doesn't fall
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Dynamic; // Must be Dynamic for triggers to work
        
        // Setup Collider as TRIGGER (enemy layer doesn't physically collide with player layer)
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError("FlyerBoss: No Collider2D found! Adding CircleCollider2D...");
            myCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
        // Must be a trigger since enemy/player layers don't physically collide
        myCollider.isTrigger = true;
        
        Debug.Log($"FlyerBoss Setup - Layer: {LayerMask.LayerToName(gameObject.layer)}, Tag: {gameObject.tag}, Trigger: {myCollider.isTrigger}");
        
        // Find player and set direction based on relative position
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            float playerX = playerObj.transform.position.x;
            
            // If boss is left of player, move right (1). If boss is right of player, move left (-1).
            direction = transform.position.x < playerX ? 1 : -1;
            
            Collider2D playerCollider = playerObj.GetComponent<Collider2D>();
            Debug.Log($"Player found - Layer: {LayerMask.LayerToName(playerObj.layer)}, Tag: {playerObj.tag}, Collider: {playerCollider != null}, IsTrigger: {playerCollider?.isTrigger}");
            
            // Check if layers can interact
            int flyerLayer = gameObject.layer;
            int playerLayer = playerObj.layer;
            bool canCollide = !Physics2D.GetIgnoreLayerCollision(flyerLayer, playerLayer);
            Debug.Log($"Layer collision check - Flyer layer {flyerLayer} vs Player layer {playerLayer}: Can collide/trigger = {canCollide}");
        }
    }

    void Update()
    {
        // Apply velocity
        rb.linearVelocity = new Vector2(direction * speed, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"FlyerBoss TRIGGER with: {other.gameObject.name}, tag: '{other.gameObject.tag}'");
        
        // Check if we triggered with the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("FlyerBoss: Player trigger detected!");
            Player_Stats playerStats = other.GetComponent<Player_Stats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
                Debug.Log($"FlyerBoss hit player for {damage} damage!");
            }
            else
            {
                Debug.LogError("FlyerBoss: Player_Stats component not found on player!");
            }
            
            // Destroy the flying boss after hitting player
            Destroy(gameObject);
        }
    }
}
            
