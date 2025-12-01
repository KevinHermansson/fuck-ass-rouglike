using UnityEngine;

public class BossHeartSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject heartPrefab; // Optional: drag heart prefab, or use child
    
    [Header("Movement Settings")]
    public float disappearDuration = 0.5f; // Time the heart stays invisible
    
    private Transform heartTransform;
    private Bounds squareBounds;
    private SpriteRenderer heartRenderer;

    void Start()
    {
        // Calculate bounds from EdgeCollider2D or Renderer
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        if (edgeCollider != null && edgeCollider.pointCount > 0)
        {
            // Make sure the edge collider is a trigger so nothing collides with it
            edgeCollider.isTrigger = true;
            
            // Calculate bounds from edge collider points
            Vector2[] points = edgeCollider.points;
            Vector2 worldPoint = transform.TransformPoint(points[0]);
            Vector2 min = worldPoint;
            Vector2 max = worldPoint;
            
            foreach (Vector2 point in points)
            {
                worldPoint = transform.TransformPoint(point);
                min = Vector2.Min(min, worldPoint);
                max = Vector2.Max(max, worldPoint);
            }
            
            Vector3 center = new Vector3((min.x + max.x) / 2f, (min.y + max.y) / 2f, transform.position.z);
            Vector3 size = new Vector3(max.x - min.x, max.y - min.y, 0f);
            squareBounds = new Bounds(center, size);
            Debug.Log($"Boss square bounds from EdgeCollider: {squareBounds.size}, center: {squareBounds.center}");
        }
        else
        {
            // Fallback to renderer
            Renderer squareRenderer = GetComponent<Renderer>();
            if (squareRenderer != null)
            {
                squareBounds = squareRenderer.bounds;
                Debug.Log($"Boss square bounds from Renderer: {squareBounds.size}, center: {squareBounds.center}");
            }
            else
            {
                // Fallback: use a default size
                squareBounds = new Bounds(transform.position, Vector3.one * 10f);
                Debug.LogWarning("This GameObject has no EdgeCollider or Renderer, using default bounds of 10x10");
            }
        }
        
        // Find or instantiate the heart
        if (heartPrefab != null)
        {
            GameObject heartInstance = Instantiate(heartPrefab, transform.position, Quaternion.identity);
            heartInstance.transform.parent = transform;
            heartTransform = heartInstance.transform;
        }
        else if (transform.childCount > 0)
        {
            // Use first child as heart
            heartTransform = transform.GetChild(0);
        }
        
        if (heartTransform == null)
        {
            Debug.LogWarning("No boss heart found! Make sure the heart is a child or assign heartPrefab.");
            return;
        }
        
        // Get the heart's sprite renderer
        heartRenderer = heartTransform.GetComponent<SpriteRenderer>();
        
        // Set initial random position above the boss square
        SetRandomPosition();
    }

    // Call this method to move the heart to a new position
    public void MoveHeartToNewPosition()
    {
        StartCoroutine(DisappearAndReappear());
    }

    System.Collections.IEnumerator DisappearAndReappear()
    {
        // Make heart invisible
        if (heartRenderer != null)
        {
            heartRenderer.enabled = false;
        }
        
        // Disable collider so player can't hit it while invisible
        Collider2D heartCollider = heartTransform.GetComponent<Collider2D>();
        if (heartCollider != null)
        {
            heartCollider.enabled = false;
        }
        
        // Wait
        yield return new WaitForSeconds(disappearDuration);
        
        // Move to new random position
        SetRandomPosition();
        
        // Make heart visible again
        if (heartRenderer != null)
        {
            heartRenderer.enabled = true;
        }
        
        // Enable collider
        if (heartCollider != null)
        {
            heartCollider.enabled = true;
        }
    }

    void SetRandomPosition()
    {
        if (heartTransform == null) return;
        
        // Generate random position within the square bounds
        float randomX = Random.Range(squareBounds.min.x, squareBounds.max.x);
        float randomY = Random.Range(squareBounds.min.y, squareBounds.max.y);
        
        // Clamp Y position to not go below -3
        randomY = Mathf.Max(randomY, -3f);
        
        // Make sure heart spawns above the square (in front on Z-axis)
        float heartZ = transform.position.z - 1f;
        
        heartTransform.position = new Vector3(randomX, randomY, heartZ);
        
        Debug.Log($"Boss heart positioned at: {heartTransform.position}");
    }

    // Visualize the square area in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        
        Renderer squareRenderer = GetComponent<Renderer>();
        if (squareRenderer != null)
        {
            Gizmos.DrawWireCube(squareRenderer.bounds.center, squareRenderer.bounds.size);
        }
        else
        {
            // Show default bounds if no renderer
            Gizmos.DrawWireCube(transform.position, Vector3.one * 10f);
        }
    }
}
