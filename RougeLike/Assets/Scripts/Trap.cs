using UnityEngine;

public class Trap : MonoBehaviour
{
    public GameObject spikes; // Assign the spikes object in Inspector
    public float spikeDropForce = 10f;
    private bool triggered = false;

    void Start()
    {
        if (spikes != null)
            spikes.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            if (spikes != null)
            {
                spikes.SetActive(true);
                Rigidbody2D rb = spikes.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.linearVelocity = Vector2.down * spikeDropForce;
                }
                // Attach the spike collision handler
                if (spikes.GetComponent<SpikeCollisionHandler>() == null)
                    spikes.AddComponent<SpikeCollisionHandler>();
            }
        }
    }
}

// Handles spike collision logic
public class SpikeCollisionHandler : MonoBehaviour
{
    private bool hasHit = false;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;
        if (collision.collider.CompareTag("Player"))
        {
            Player_Stats stats = collision.collider.GetComponent<Player_Stats>();
            if (stats != null)
            {
                stats.TakeDamage(50);
                Destroy(gameObject);
            }
            hasHit = true;
        }
        else if (collision.collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }


}
