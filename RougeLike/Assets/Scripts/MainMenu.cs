using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the first scene of the game
        UnityEngine.SceneManagement.SceneManager.LoadScene("Hub");
    }

    public void tutorial()
    {
        // Load the tutorial scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("roomSamples");
    }
}
