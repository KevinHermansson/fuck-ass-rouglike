using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class healthbar_player : MonoBehaviour
{
    public Image healthBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Input.GetKey(KeyCode.E))
        {
            healthBar.fillAmount = 0.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
