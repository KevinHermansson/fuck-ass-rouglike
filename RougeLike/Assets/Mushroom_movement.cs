using UnityEngine;

public class Mushroom_movement : MonoBehaviour
{
     public float jumpHeight = 8;
    public float moveSpeed = 5;
    public Rigidbody2D rb;


    public float groundCheck = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.Rigidbody2D.position.x - rb.position.x < AwakeDistance)
        {
            // Move towards the player
            Vector2 direction = (player.Rigidbody2D.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
        }
    }
}
