using UnityEngine;

public class MinibossBoundary : MonoBehaviour
{
    public Miniboss_Movement miniboss; // Drag the miniboss here in the Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Boundary trigger entered by: {collision.gameObject.name}, Tag: {collision.tag}");
        
        // Check if the player entered the boundary - activate miniboss
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player detected!");
            
            if (miniboss != null)
            {
                Debug.Log("Miniboss reference found - activating!");
                miniboss.ActivateMiniboss();
                
                // Show miniboss health bar
                MinibossHealth healthScript = miniboss.GetComponent<MinibossHealth>();
                if (healthScript != null)
                {
                    healthScript.ShowHealthBar();
                    Debug.Log("Health bar shown!");
                }
                else
                {
                    Debug.LogError("MinibossHealth script not found!");
                }
            }
            else
            {
                Debug.LogError("Miniboss reference is NULL! Drag the miniboss into the Inspector!");
            }
        }
    }


}
