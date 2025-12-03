using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    private bool ePressed = false;
    
    void Update()
    {
        // Check if Game Over UI is visible and E is pressed (works even when time is frozen)
        if (gameOverUI != null && gameOverUI.activeInHierarchy)
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
