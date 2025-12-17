using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    private bool ePressed = false;
    private static GameManager instance;

    void Awake()
    {
        // Singleton pattern - keep only one GameManager across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager created and persistent");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Find and setup reset button automatically
        SetupResetButton();
    }

    void OnLevelWasLoaded(int level)
    {
        // Re-setup button when scene changes
        SetupResetButton();
    }

    void SetupResetButton()
    {
        // Try to find the GameOverUI if not assigned
        if (gameOverUI == null)
        {
            gameOverUI = GameObject.Find("GameOverUI");
        }

        // Find and setup the reset button
        Button resetButton = GameObject.Find("ResetButton")?.GetComponent<Button>();
        if (resetButton != null)
        {
            // Clear existing listeners and add new one
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(RestartGame);
            Debug.Log("Reset button configured!");
        }
    }

    void Update()
    {
        // Check if Game Over UI is visible and E is pressed
        GameObject currentGameOverUI = gameOverUI ?? GameObject.Find("GameOverUI");

        if (currentGameOverUI != null && currentGameOverUI.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.E) && !ePressed)
            {
                ePressed = true;
                RestartGame();
            }
        }
        else
        {
            ePressed = false;
        }
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame called!");

        // Reset time scale immediately so coroutine can run
        Time.timeScale = 1f;

        // Hide game over UI immediately
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }



        // Reset player instead of destroying
        Player_Stats playerStats = FindFirstObjectByType<Player_Stats>();
        if (playerStats != null)
        {
            // Reset position to spawn point
            playerStats.transform.position = new Vector3(0, 0, 0);

            // Reset health to max (preserving upgrades)
            playerStats.health = playerStats.MaxHealth;


            // Update health bar to reflect full health
            if (playerStats.healthBar != null)
            {
                playerStats.healthBar.fillAmount = playerStats.health / playerStats.MaxHealth;
            }

            // Reset death state so player can die again
            playerStats.GetType().GetField("isDead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(playerStats, false);

            // Re-enable movement
            MovementScript movementScript = playerStats.GetComponent<MovementScript>();
            if (movementScript != null)
            {
                movementScript.enabled = true;
            }

            // Reset rigidbody
            Rigidbody2D rb = playerStats.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

            // Make sprite visible again
            SpriteRenderer spriteRenderer = playerStats.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }

            Debug.Log("Player reset with health: " + playerStats.health + "/" + playerStats.MaxHealth);
        }

        // Load the hub scene immediately
        SceneManager.LoadScene("Hub");
    }

    IEnumerator RestartCoroutine()
    {
        // No longer used - keeping for compatibility
        yield return null;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
