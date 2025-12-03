using UnityEngine;

public class HubSceneManager : MonoBehaviour
{
    void Start()
    {
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
