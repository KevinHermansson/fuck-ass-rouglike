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
        
        // Destroy all DontDestroyOnLoad objects
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Debug.Log("Destroying: " + obj.name);
                Destroy(obj);
            }
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
