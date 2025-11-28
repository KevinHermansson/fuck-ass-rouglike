using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBackgroundManager : MonoBehaviour
{
    void Start()
    {
        // Check which scene is loaded and set camera background color accordingly
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "BeginningScene")
        {
            Camera.main.backgroundColor = new Color(0x2b / 255f, 0x7f / 255f, 0xa6 / 255f); // #2b7fa6
        }
    }
}
