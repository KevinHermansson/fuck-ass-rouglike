using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private int damage;
    private LayerMask enemyLayers;
    private float lifetime;
    private bool hasHit = false;
    
    public void Initialize(int dmg, LayerMask layers, float life)
    {
        damage = dmg;
        enemyLayers = layers;
        lifetime = life;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        
        // Check if we hit an enemy
        if (((1 << other.gameObject.layer) & enemyLayers) != 0)
        {
            hasHit = true;
            
            Enemy_Health enemyHealth = other.GetComponent<Enemy_Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            
            MinibossHealth bossHealth = other.GetComponent<MinibossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
            }
            
            // Destroy projectile on hit
            Destroy(gameObject);
        }
        // Also destroy on hitting walls/ground
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}

