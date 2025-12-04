using UnityEngine;

public class FlyingBat_movement : MonoBehaviour
{
    public float speed = 3f;
    public float edgeBuffer = 1f; // Distance from camera edge to turn around
    public GameObject stonePrefab; // Assign the stone prefab in the Inspector
    public float minDropInterval = 2f; // Minimum time between stone drops
    public float maxDropInterval = 5f; // Maximum time between stone drops

    private Camera mainCamera;
    private bool movingRight = true;
    private float leftBound;
    private float rightBound;
    private float dropTimer = 0f;
    private float currentDropInterval;

    public Rigidbody2D rb;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {

            return;
        }

        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
        }

        // Make sure Rigidbody2D is set to Dynamic for collision detection
        if (rb != null && rb.bodyType != RigidbodyType2D.Dynamic)
        {

        }

        // Check if bat has a collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Ignore collisions with all other enemies (including other bats)
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy != gameObject) // Don't ignore collision with self
            {
                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
                if (enemyCollider != null && collider != null)
                {
                    Physics2D.IgnoreCollision(collider, enemyCollider);
                }
            }
        }

        // Ignore collision with player
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            Collider2D playerCollider = p.GetComponent<Collider2D>();
            if (playerCollider != null && collider != null)
            {
                Physics2D.IgnoreCollision(collider, playerCollider);
            }
        }

        // Ignore collisions with all stones
        GameObject[] stones = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject stone in stones)
        {
            Collider2D stoneCollider = stone.GetComponent<Collider2D>();
            if (stoneCollider != null && collider != null)
            {
                Physics2D.IgnoreCollision(collider, stoneCollider);
            }
        }

        // Calculate camera bounds in world space
        UpdateCameraBounds();

        // Set initial random drop interval
        currentDropInterval = Random.Range(minDropInterval, maxDropInterval);
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
        if (dropTimer >= currentDropInterval)
        {
            DropStone();
            dropTimer = 0f;
            // Set new random interval for next drop
            currentDropInterval = Random.Range(minDropInterval, maxDropInterval);
        }
    }

    private void UpdateCameraBounds()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return; // Still null, exit
        }

        // Get camera bounds at the bat's Y position
        float batY = transform.position.y;
        float camDistance = mainCamera.transform.position.z - batY;

        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, camDistance));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, camDistance));

        leftBound = leftEdge.x + edgeBuffer;
        rightBound = rightEdge.x - edgeBuffer;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {


        string tag = collision.gameObject.tag;

        // Ignore collisions with enemies completely
        if (tag == "enemy")
        {

            return;
        }

        if (tag == "fancyPlatform" || tag == "Wall" || tag == "Ground")
        {
            movingRight = !movingRight;


            // Move bat slightly away from collision to prevent getting stuck
            Vector2 pushDirection = movingRight ? Vector2.right : Vector2.left;
            rb.MovePosition(rb.position + pushDirection * 0.2f);
        }
    }


    public void DropStone()
    {
        if (stonePrefab == null)
        {

            return;
        }

        // Spawn stone right below the bat
        Vector3 dropPosition = transform.position + Vector3.down * 0.9f; // Adjust offset as needed
        GameObject stone = Instantiate(stonePrefab, dropPosition, Quaternion.identity);

        // Ignore collision between this bat and the newly spawned stone
        Collider2D batCollider = GetComponent<Collider2D>();
        Collider2D stoneCollider = stone.GetComponent<Collider2D>();

        if (batCollider != null && stoneCollider != null)
        {
            Physics2D.IgnoreCollision(batCollider, stoneCollider);
        }


    }
}
