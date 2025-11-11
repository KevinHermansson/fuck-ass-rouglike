using UnityEngine;

public class FlyingBat_movement : MonoBehaviour
{
    public float speed = 3f;
    public float edgeBuffer = 1f; // Distance from camera edge to turn around
    public GameObject stonePrefab; // Assign the stone prefab in the Inspector
    public float dropInterval = 3f; // Time between stone drops in seconds
    
    private Camera mainCamera;
    private bool movingRight = true;
    private float leftBound;
    private float rightBound;
    private float dropTimer = 0f;

    public Rigidbody2D rb;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("No main camera found!");
            return;
        }
        
        rb = GetComponent<Rigidbody2D>();
        // Calculate camera bounds in world space
        UpdateCameraBounds();
    }

    void FixedUpdate()
    {
        if (mainCamera == null || rb == null) return;
        
        // Update bounds (in case camera moves)
        UpdateCameraBounds();
        
        // Move bat using Rigidbody2D
        if (movingRight)
        {
            rb.MovePosition(rb.position + Vector2.right * speed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            
            // Check if reached right edge
            if (rb.position.x >= rightBound)
            {
                movingRight = false;
            }
        }
        else
        {
            rb.MovePosition(rb.position + Vector2.left * speed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Euler(0, 180, 0);
            
            // Check if reached left edge
            if (rb.position.x <= leftBound)
            {
                movingRight = true;
            }
        }

        // Handle stone dropping timer
        dropTimer += Time.fixedDeltaTime;
        if (dropTimer >= dropInterval)
        {
            DropStone();
            dropTimer = 0f;
        }
    }
    
    private void UpdateCameraBounds()
    {
        // Get camera bounds at the bat's Y position
        float batY = transform.position.y;
        float camDistance = mainCamera.transform.position.z - batY;
        
        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, camDistance));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, camDistance));
        
        leftBound = leftEdge.x + edgeBuffer;
        rightBound = rightEdge.x - edgeBuffer;
    }

    // Detect collision and change direction
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Change direction when hitting something
        movingRight = !movingRight;
        Debug.Log($"Bat collided with {collision.gameObject.name}, changing direction!");
    }

    // Function to spawn a stone sprite
    public void DropStone()
    {
        if (stonePrefab == null)
        {
            Debug.LogWarning("Stone prefab is not assigned!");
            return;
        }

        // Spawn stone right below the bat
        Vector3 dropPosition = transform.position + Vector3.down * 0.5f; // Adjust offset as needed
        GameObject stone = Instantiate(stonePrefab, dropPosition, Quaternion.identity);
        Debug.Log("Stone dropped!");
    }
}
