using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
    public float jumpHeight = 8;
    public float moveSpeed = 5;
    public Rigidbody2D rb;


    public float groundCheck = 0;
    public float wallCheck = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // jump once when pressed
        if (Input.GetKeyDown(KeyCode.Space) && groundCheck >= 1)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        }

        if (Input.GetKeyDown(KeyCode.Space) && wallCheck >= 1)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        }

        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);

        float vx = 0f;
        if (left && !right)
        {
            vx = -moveSpeed;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (right && !left)
        {
            vx = moveSpeed;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // both pressed (or neither) -> stop horizontal movement
            vx = 0f;
        }

        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
    }

    // set groundCheck when colliding with objects tagged "Ground"
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            groundCheck += 1;

        if (collision.collider.CompareTag("Wall"))
            wallCheck += 1;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            groundCheck -= 1;

        if (collision.collider.CompareTag("Wall"))
            wallCheck -= 1;
    }


}
