using UnityEngine;

public class MinibossActivationTrigger : MonoBehaviour
{
    public Miniboss_Movement miniboss; // Assign the miniboss in the Inspector
    public GameObject minibossHealthBar; // Assign the health bar UI in the Inspector

    private void Start()
    {
        // Hide health bar at start
        if (minibossHealthBar != null)
        {
            minibossHealthBar.SetActive(false);
        }
    }

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
            
            // Show health bar when player enters
            if (minibossHealthBar != null)
            {
                minibossHealthBar.SetActive(true);
            }
        }
    }
}
