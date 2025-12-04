using UnityEngine;

public class HubSceneManager : MonoBehaviour
{
    void Start()
    {
        // Set camera background to brown
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = new Color(0.6f, 0.4f, 0.2f); // Brown color
        }

        // Find and hide Game Over UI
        GameObject gameOverUI = GameObject.Find("GameOverUI");
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // Find and show Player Health Bar
        GameObject healthBar = GameObject.Find("PlayerHealthBar");
        if (healthBar != null)
        {
            healthBar.SetActive(true);
        }

        // Check for PebbleManager
        if (PebbleManager.Instance != null)
        {
            PebbleManager.Instance.gameObject.SetActive(true);
            Debug.Log("Hub: PebbleManager Instance found and activated");
        }
        else
        {
            // Try to find in scene
            GameObject pebbleManager = GameObject.Find("PebbleManager");
            if (pebbleManager != null)
            {
                pebbleManager.SetActive(true);
                Debug.Log("Hub: Found PebbleManager in scene and activated");
            }
            else
            {
                Debug.LogWarning("Hub: No PebbleManager found! It should exist in the first scene that loads, or be in this scene for testing.");
            }
        }

        Debug.Log("Hub scene initialized - Game Over hidden, Health Bar and Pebble Manager visible");
    }
}
