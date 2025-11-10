using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MovementScript : MonoBehaviour
{
    public float jumpHeight = 8;
    public float moveSpeed = 5;
    public Rigidbody2D rb;


    public float groundCheck = 1;
    public bool fancyGroundCheck;
    BoxCollider2D PlayerCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // use rb.velocity (Rigidbody2D) and Vector2
        if (Input.GetKey(KeyCode.Space) && groundCheck == 1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        } if (Input.GetKey(KeyCode.S)){
            dropThroughPlatform();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
        }


    }

    // set groundCheck when colliding with objects tagged "Ground"
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            groundCheck = 1;
        if (collision.collider.CompareTag("fancyPlatform"))
            fancyGroundCheck = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            groundCheck = 0;
        if (collision.collider.CompareTag("fancyPlatform"))
            fancyGroundCheck = false;
    }

    private IEnumerator DisablePlayerCollider(float disableTime){
        PlayerCollider.enabled = false;
        yield return new WaitForSeconds(disableTime);
        PlayerCollider.enabled = true;
    }

    public void dropThroughPlatform()
    {
        if (Input.GetKey(KeyCode.S) && fancyGroundCheck && PlayerCollider.enabled){
            StartCoroutine(DisablePlayerCollider(0.5f));
        }
    }
}
