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

        // Find and show Pebble Manager
        GameObject pebbleManager = GameObject.Find("PebbleManager");
        if (pebbleManager != null)
        {
            pebbleManager.SetActive(true);
        }

        Debug.Log("Hub scene initialized - Game Over hidden, Health Bar and Pebble Manager visible");
    }
}
