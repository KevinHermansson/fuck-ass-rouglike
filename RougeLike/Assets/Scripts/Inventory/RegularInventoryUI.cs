using UnityEngine;
using UnityEngine.UI;

public class RegularInventoryUI : MonoBehaviour
{
    [SerializeField] private RegularInventory inventory;
    [SerializeField] private Transform slotRoot;
    [SerializeField] private Color selectedTint = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color unselectedTint = new Color(1f, 1f, 1f, 0.4f);

    private Image[] icons;

    private void OnEnable()
    {
        CacheChildren();
        if (inventory != null) inventory.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null) inventory.OnChanged -= Refresh;
    }

    private void CacheChildren()
    {
        int count = slotRoot.childCount;
        icons = new Image[count];

        for (int i = 0; i < count; i++)
        {
            var child = slotRoot.GetChild(i);
            var iconTr = child.Find("Icon");
            if (iconTr == null)
            {
                Debug.LogError($"[RegularInventoryUI] Slot {i} is missing a child named 'Icon'");
                continue;
            }
            icons[i] = iconTr.GetComponent<Image>();
            var btn = child.GetComponent<Button>();
            int captured = i;
            if (btn) btn.onClick.AddListener(() => { inventory.SelectedIndex = captured; Refresh(); });
        }
    }

    private void Refresh()
    {
        if (inventory == null || icons == null) return;

        for (int i = 0; i < RegularInventory.Capacity && i < icons.Length; i++)
        {
            var item = inventory.GetAt(i);
            var img = icons[i];
            if (!img) continue;

            img.sprite = item ? item.Icon : null;
            img.enabled = item != null;
            img.color = (i == inventory.SelectedIndex) ? selectedTint :
                        (item ? Color.white : unselectedTint);
        }
    }

    public void SetInventory(RegularInventory inv)
    {
        if (inventory != null) inventory.OnChanged -= Refresh;
        inventory = inv;
        if (isActiveAndEnabled && inventory != null)
        {
            inventory.OnChanged += Refresh;
            Refresh();
        }
    }
}

