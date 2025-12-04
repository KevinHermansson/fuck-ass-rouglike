using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;


public class Startgame : MonoBehaviour
{
    public GameObject startGamePanel;
    public GameObject Miniboss;
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
        if (playerInRange3 && Input.GetKeyDown(KeyCode.E))
        {
            BossRoom();
        }
    }

    public bool playerInRange2;
    public bool playerInRange3;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            if (SceneManager.GetActiveScene().name == "BeginningScene")
            {
                if (Miniboss == null)
                {
                    playerInRange3 = true;
                }
            }
            else if (SceneManager.GetActiveScene().name == "Hub")
            {
                playerInRange2 = true;
            }
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

    public void BossRoom()
    {
        // Find the player and set spawn position
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(130, 70, 0);
        }
        SceneManager.LoadScene("BossScene");
    }


}
