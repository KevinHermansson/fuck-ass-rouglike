using UnityEngine;
using UnityEngine.SceneManagement;

public class panelManager : MonoBehaviour
{
    public GameObject Dia;
    public GameObject Upg;
    public GameObject UpgB;
    public GameObject DiaB;
    public GameObject Sta;
    public bool isActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        setActive();
    }
    public void setActive()
    {
        if (SceneManager.GetActiveScene().name == "Hub" && !isActive)
        {
            Dia.SetActive(true);
            Upg.SetActive(true);
            UpgB.SetActive(true);
            DiaB.SetActive(true);
            Sta.SetActive(true);
            Debug.Log("Panels Activated");
            isActive = true;
        }
        else if (SceneManager.GetActiveScene().name != "Hub" && isActive)
        {
            isActive = false;
        }
    }
}
