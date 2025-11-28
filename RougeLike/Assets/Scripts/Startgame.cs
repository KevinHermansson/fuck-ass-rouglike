using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class Startgame : MonoBehaviour
{
    public GameObject startGamePanel;
    void Update()
    {
        if (playerInRange2 && Input.GetKeyDown(KeyCode.E))
        {
            if (startGamePanel.activeInHierarchy)
            {
                startGamePanel.SetActive(false);
            }
            else
                startGamePanel.SetActive(true);
            Debug.Log("Start Game Triggered");
        }
    }

    public bool playerInRange2;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange2 = true;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange2 = false;
            startGamePanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("BeginningScene");
    }
}
