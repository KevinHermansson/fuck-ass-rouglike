using UnityEngine;
using TMPro;

public class BossDeath : MonoBehaviour
{
    public TextMeshProUGUI bossHPText;
    private bool hasRotated = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        int bossHP = GetBossHP();

        if (bossHP <= 0 && !hasRotated)
        {
            RotateBoss();
            hasRotated = true;
        }
    }

    void RotateBoss()
    {
        // Get sprite bounds
        if (spriteRenderer != null)
        {
            Bounds bounds = spriteRenderer.bounds;
            
            // Calculate the down-right corner position in world space
            Vector3 pivotPoint = new Vector3(bounds.max.x, bounds.min.y, transform.position.z);
            
            // Calculate offset from current position to pivot point
            Vector3 offset = transform.position - pivotPoint;
            
            // Rotate the boss 45 degrees to the right (clockwise)
            transform.Rotate(0, 0, -45f);
            
            // Apply rotation to offset and reposition
            Vector3 rotatedOffset = Quaternion.Euler(0, 0, -45f) * offset;
            transform.position = pivotPoint + rotatedOffset;
        }
        else
        {
            // Fallback: just rotate in place if no sprite renderer
            transform.Rotate(0, 0, -45f);
        }
    }

    int GetBossHP()
    {
        // Try to read from the text component first
        if (bossHPText != null && !string.IsNullOrEmpty(bossHPText.text))
        {
            if (int.TryParse(bossHPText.text, out int hp))
            {
                return hp;
            }
        }

        // Fallback: Find the BossHeart component in the scene
        BossHeart bossHeart = FindObjectOfType<BossHeart>();
        if (bossHeart != null)
        {
            return bossHeart.GetCurrentHeartCount();
        }

        return 10; // Default value if nothing found
    }
}
