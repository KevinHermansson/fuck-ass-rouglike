using UnityEngine;
using UnityEngine.UI;

public class Player_Health : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;
    public Image healthBar;


    void Start()
    {
        health = maxHealth;
        
        if (healthBar == null)
        {
            Debug.LogError("healthBar Image is not assigned in Inspector!");
        }
        else
        {
            Debug.Log($"healthBar assigned. Type: {healthBar.type}");
            healthBar.fillAmount = 1f; // full at start
        }
    }

    void Update()
    {
        // clamp health between 0 and maxHealth
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        float fillValue = health / maxHealth;
        Debug.Log($"Player took {damageAmount} damage. Health: {health}, fillAmount: {fillValue}");
        
        if (healthBar != null)
        {
            healthBar.fillAmount = fillValue;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Add death logic here (respawn, game over, etc.)
    }
}
