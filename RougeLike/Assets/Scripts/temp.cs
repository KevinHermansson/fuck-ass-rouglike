using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class temp : MonoBehaviour
{
    bool yuh = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            toHub();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.CompareTag("player"))
        {
            yuh = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.CompareTag("player"))
        {
            yuh = false;
        }
    }

    public void toHub()
    {
        SceneManager.LoadScene("Hub");
    }
}
