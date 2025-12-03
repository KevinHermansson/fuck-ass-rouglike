using UnityEngine;

public class LevelSceneManager : MonoBehaviour
{
    void Start()
    {
        // Find and hide Game Over UI
        GameObject gameOverUI = GameObject.Find("GameOverUI");
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        Debug.Log("Level scene initialized - Game Over UI hidden");
    }
}
