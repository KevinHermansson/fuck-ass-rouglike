using UnityEngine;

public class Minibossball_Behavior : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 10f;
    public float lifetime = 50f; // Max lifetime to prevent endless balls

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        // Find the player in the scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found for Minibossball_Behavior!");
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on Minibossball!");
            return;
        }

        Destroy(gameObject, lifetime); // Destroy after a certain lifetime
    }

    void FixedUpdate()
    {
        if (player != null && rb != null)
        {
            // Calculate direction towards the player
            Vector2 direction = (player.position - transform.position).normalized;
            // Set the velocity to move towards the player
            rb.linearVelocity = direction * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore triggers with enemies
        if (other.CompareTag("enemy"))
        {
            return;
        }

        // Check if the ball hits the player
        if (other.CompareTag("Player"))
        {
            Player_Stats playerHealth = other.GetComponent<Player_Stats>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Minibossball hit player! Dealt {damage} damage.");
            }
            else
            {
                Debug.LogWarning("Player_Stats component not found on player!");
            }
        }
        
        // Destroy the ball if it hits the player or any other object (like a wall)
        // This prevents the ball from flying through walls.
        Destroy(gameObject);
    }
}
