using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RegularInventoryUI : MonoBehaviour
{
    [SerializeField] private RegularInventory inventory;
    public RegularInventory Inventory => inventory;
    [SerializeField] private Transform slotRoot;
    [SerializeField] private Color selectedTint = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color unselectedTint = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private Color selectedBackgroundColor = new Color(0f, 0.8f, 1f, 0.3f);
    [SerializeField] private KeyCode dropKey = KeyCode.X;
    [SerializeField] private GameObject itemDropPrefab;

    private Image[] icons;
    private Button[] buttons;
    private Image[] slotBackgrounds;
    private bool isCached = false;
    private Transform playerTransform;

    private void OnEnable()
    {
        if (!isCached) CacheChildren();
        if (inventory != null) inventory.OnChanged += Refresh;
        Refresh();
        
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        
        if (InventorySelectionManager.Instance != null)
        {
            InventorySelectionManager.Instance.RegisterRegularUI(this);
        }
    }

    private void OnDisable()
    {
        if (inventory != null) inventory.OnChanged -= Refresh;
    }

    private void Update()
    {
        if (Input.GetKeyDown(dropKey) && inventory != null && isActiveAndEnabled)
        {
            if (transform.root.gameObject.activeSelf)
            {
                bool mouseOverThis = IsMouseOverInventory();
                if (mouseOverThis)
                {
                    int selectedIndex = inventory.SelectedIndex;
                    var item = inventory.GetAt(selectedIndex);
                    if (item != null)
                    {
                        DropSelectedItem();
                    }
                }
            }
        }
    }
    
    private bool IsMouseOverInventory()
    {
        if (slotRoot == null) return false;
        
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        foreach (var result in results)
        {
            if (result.gameObject.transform.IsChildOf(slotRoot) || result.gameObject.transform == slotRoot)
            {
                return true;
            }
        }
        return false;
    }

    private void CacheChildren()
    {
        if (slotRoot == null) return;
        
        int count = slotRoot.childCount;
        icons = new Image[count];
        buttons = new Button[count];
        slotBackgrounds = new Image[count];

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
            
            buttons[i] = child.GetComponent<Button>();
            if (buttons[i] != null)
            {
                buttons[i].onClick.RemoveAllListeners();
                
                var eventTrigger = buttons[i].gameObject.GetComponent<EventTrigger>();
                if (eventTrigger == null)
                    eventTrigger = buttons[i].gameObject.AddComponent<EventTrigger>();
                
                var entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                int captured = i;
                entry.callback.AddListener((data) => {
                    var pointerEventData = data as PointerEventData;
                    if (pointerEventData != null && pointerEventData.button == PointerEventData.InputButton.Right)
                    {
                        DropItemAt(captured);
                    }
                });
                eventTrigger.triggers.Add(entry);
                
                int capturedIndex = i;
                buttons[i].onClick.AddListener(() => {
                    if (inventory != null)
                    {
                        InventorySelectionManager.Instance.SelectRegular(capturedIndex);
                    }
                });
            }
            
            var bgTr = child.Find("Background");
            if (bgTr == null)
            {
                GameObject bgObj = new GameObject("Background");
                bgObj.transform.SetParent(child, false);
                RectTransform bgRect = bgObj.AddComponent<RectTransform>();
                bgRect.anchorMin = Vector2.zero;
                bgRect.anchorMax = Vector2.one;
                bgRect.sizeDelta = Vector2.zero;
                bgRect.anchoredPosition = Vector2.zero;
                
                Image bgImg = bgObj.AddComponent<Image>();
                bgImg.color = Color.clear;
                slotBackgrounds[i] = bgImg;
            }
            else
            {
                slotBackgrounds[i] = bgTr.GetComponent<Image>();
                if (slotBackgrounds[i] == null)
                    slotBackgrounds[i] = bgTr.gameObject.AddComponent<Image>();
            }
        }
        
        isCached = true;
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
            
            if (slotBackgrounds != null && i < slotBackgrounds.Length && slotBackgrounds[i] != null)
            {
                if (i == inventory.SelectedIndex && item != null)
                {
                    slotBackgrounds[i].color = selectedBackgroundColor;
                }
                else
                {
                    slotBackgrounds[i].color = Color.clear;
                }
            }
        }
    }

    private void DropSelectedItem()
    {
        if (inventory == null) return;
        
        int selectedIndex = inventory.SelectedIndex;
        DropItemAt(selectedIndex);
    }

    private void DropItemAt(int index)
    {
        if (inventory == null) return;
        
        var item = inventory.GetAt(index);
        if (item != null && item is RegularItem regularItem)
        {
            inventory.RemoveAt(index);
            
            if (itemDropPrefab != null && playerTransform != null)
            {
                Vector3 dropPosition = playerTransform.position + Vector3.up * 0.5f;
                GameObject droppedItem = Instantiate(itemDropPrefab, dropPosition, Quaternion.identity);
                
                var pickup = droppedItem.GetComponent<RegularItemPickup>();
                if (pickup != null)
                {
                    pickup.itemToGive = regularItem;
                    pickup.useProximityPickup = true;
                }
                
                SpriteRenderer spriteRenderer = droppedItem.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = droppedItem.AddComponent<SpriteRenderer>();
                }
                
                if (item.Icon != null && spriteRenderer.sprite == null)
                {
                    spriteRenderer.sprite = item.Icon;
                }
                
                spriteRenderer.enabled = true;
                
            }
            else
            {
                Debug.LogWarning("Cannot drop item: Missing drop prefab or player transform");
            }
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
