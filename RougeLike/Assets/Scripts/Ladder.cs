using UnityEngine;

public class Ladder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MovementScript movementScript = other.GetComponent<MovementScript>();
            if (movementScript != null)
            {
                movementScript.wallCheck += 1;
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MovementScript movementScript = other.GetComponent<MovementScript>();
            if (movementScript != null)
            {
                movementScript.wallCheck -= 1;
            }
        }
    }
}


