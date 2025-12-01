using UnityEngine;

public class MinibossActivationTrigger : MonoBehaviour
{
    public Miniboss_Movement miniboss; // Assign the miniboss in the Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player entered the trigger
        if (collision.CompareTag("Player"))
        {
            if (miniboss != null)
            {
                miniboss.ActivateMiniboss();
                Debug.Log("Player entered miniboss activation zone!");
            }
        }
    }
}
