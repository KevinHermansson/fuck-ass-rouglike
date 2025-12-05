using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BossHeart : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 10f;
    public float maxHealth = 10f;
    
    [Header("Animation Settings")]
    public float pulseScale = 1.3f; // How much bigger the heart gets
    public float pulseDuration = 0.2f; // Total duration of the pulse animation
    
    [Header("UI Settings")]
   public TextMeshProUGUI heartCounterText; // Drag your TextMeshPro - Text (UI) element here
    public int startingHeartCount = 10; // Starting number of hearts
    
    private BossHeartSpawner spawner;
    private Vector3 originalScale;
    private bool isPulsing = false;
    private int currentHeartCount;

    void Start()
    {
        // Find the spawner (should be parent)
        spawner = GetComponentInParent<BossHeartSpawner>();
        if (spawner == null)
        {
            Debug.LogWarning("BossHeart: No BossHeartSpawner found in parent!");
        }
        
        // Make sure the collider is a trigger so player doesn't physically collide
        Collider2D heartCollider = GetComponent<Collider2D>();
        if (heartCollider != null)
        {
            heartCollider.isTrigger = true;
        }
        
        // Store original scale
        originalScale = transform.localScale;
        
        // Try to find the text if not assigned
        if (heartCounterText == null)
        {
            GameObject bossHPObj = GameObject.Find("BossHP");
            if (bossHPObj != null)
            {
                heartCounterText = bossHPObj.GetComponent<TextMeshProUGUI>();
                if (heartCounterText != null)
                {
                    Debug.Log("Found heart counter text automatically!");
                }
            }
        }
        
        // Initialize heart counter
        currentHeartCount = startingHeartCount;
        UpdateHeartCounterUI();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        
        Debug.Log($"BossHeart took {damage} damage! Health: {health}/{maxHealth}");
        
        // Play pulse animation
        if (!isPulsing)
        {
            StartCoroutine(PulseAnimation());
        }
        
        // Check if health reached 0
        if (health <= 0)
        {
            TeleportAndReset();
        }
    }

    IEnumerator PulseAnimation()
    {
        isPulsing = true;
        
        float halfDuration = pulseDuration / 2f;
        float elapsed = 0f;
        
        // Scale up
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * pulseScale, t);
            yield return null;
        }
        
        elapsed = 0f;
        
        // Scale back down
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale * pulseScale, originalScale, t);
            yield return null;
        }
        
        // Ensure we're back to original scale
        transform.localScale = originalScale;
        isPulsing = false;
    }

    void TeleportAndReset()
    {
        Debug.Log("Boss Heart health depleted! Teleporting...");
        
        // Decrease heart counter
        currentHeartCount--;
        UpdateHeartCounterUI();
        
        // Check if all hearts are depleted
        if (currentHeartCount <= 0)
        {
            Die();
            return;
        }
        
        // Reset health to max
        health = maxHealth;
        
        // Notify spawner to move the heart
        if (spawner != null)
        {
            spawner.MoveHeartToNewPosition();
        }
    }
    
    void UpdateHeartCounterUI()
    {
        if (heartCounterText != null)
        {
            heartCounterText.text = currentHeartCount.ToString();
            Debug.Log($"Updated heart counter UI to: {currentHeartCount}");
        }
        else
        {
            Debug.LogError("Heart Counter Text is null! Make sure it's assigned in the Inspector.");
        }
    }
    
    public int GetCurrentHeartCount()
    {
        return currentHeartCount;
    }
    
    void Die()
    {
        Debug.Log("Boss Heart defeated! All hearts depleted!");
        
        // Freeze the game
        Time.timeScale = 0f;
        Debug.Log("Game frozen! Time.timeScale set to 0");
        
        Destroy(gameObject);
    }
}
