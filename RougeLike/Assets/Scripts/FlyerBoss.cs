using UnityEngine;

public class FlyerBoss : MonoBehaviour
{
    public float speed = 3f;

    private int direction = 1; // 1 for right, -1 for left
    private Camera mainCamera;
    private float leftBound;
    private float rightBound;
    private Rigidbody2D rb;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        CalculateCameraBounds();

        // Ensure boss doesn't fall initially
        rb.gravityScale = 0;
        
        // Find player and set direction based on relative position
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            float playerX = playerObj.transform.position.x;
            // If boss is left of player, move right (1). If boss is right of player, move left (-1)
            direction = transform.position.x < playerX ? 1 : -1;
        }
        
        // Destroy after 5 seconds
        Destroy(gameObject, 5f);
    }

    void FixedUpdate()
    {
        Move();
    }

    void CalculateCameraBounds()
    {
        if (mainCamera != null)
        {
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;
            leftBound = mainCamera.transform.position.x - cameraWidth / 2f + 1f;
            rightBound = mainCamera.transform.position.x + cameraWidth / 2f - 1f;
        }
    }

    void Move()
    {
        

        // Apply velocity
        rb.linearVelocity = new Vector2(direction * speed, 0);
    }

    void FlipSprite()
    {
        // Flip the sprite by negating the X scale
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
