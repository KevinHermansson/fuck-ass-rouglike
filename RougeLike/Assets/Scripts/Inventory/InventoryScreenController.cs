using UnityEngine;

public class InventoryScreenController : MonoBehaviour
{
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject inventoryPanel;

    public void OpenInventory()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        if (hudPanel != null) hudPanel.SetActive(false);
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null || hudPanel == null) return;
        bool show = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(show);
        hudPanel.SetActive(!show);
    }
}
