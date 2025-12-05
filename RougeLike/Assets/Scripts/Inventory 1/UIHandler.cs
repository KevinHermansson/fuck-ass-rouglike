using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject inventory;

    private bool inventoryOpen = false;

    private void Start()
    {
        hud.SetActive(true);
        inventory.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryOpen = !inventoryOpen;

            hud.SetActive(!inventoryOpen);
            inventory.SetActive(inventoryOpen);
        }
    }
}
