using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SeedInventoryUI : MonoBehaviour
{
    [SerializeField] private SeedInventory inventory;
    public SeedInventory Inventory => inventory;
    [SerializeField] private Transform slotRoot;
    [SerializeField] private Color selectedTint = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color unselectedTint = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private Color selectedBackgroundColor = new Color(1f, 0.8f, 0f, 0.3f);
    [SerializeField] private KeyCode deleteKey = KeyCode.X;

    private Image[] icons;
    private Button[] buttons;
    private Image[] slotBackgrounds;
    private bool isCached = false;

    private void OnEnable()
    {
        if (!isCached) CacheChildren();
        if (inventory != null) inventory.OnChanged += Refresh;
        Refresh();
        
        if (InventorySelectionManager.Instance != null)
        {
            InventorySelectionManager.Instance.RegisterSeedUI(this);
        }
    }

    private void OnDisable()
    {
        if (inventory != null) inventory.OnChanged -= Refresh;
    }

    private void Update()
    {
        if (Input.GetKeyDown(deleteKey) && inventory != null && isActiveAndEnabled)
        {
            if (transform.root.gameObject.activeSelf)
            {
                bool mouseOverThis = IsMouseOverInventory();
                if (mouseOverThis)
                {
                    int selectedIndex = inventory.SelectedIndex;
                    var seed = inventory.GetAt(selectedIndex);
                    if (seed != null)
                    {
                        DeleteSelectedSeed();
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
        
        var results = new System.Collections.Generic.List<RaycastResult>();
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
                Debug.LogError($"[SeedInventoryUI] Slot {i} is missing a child named 'Icon'");
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
                        DeleteSeedAt(captured);
                    }
                });
                eventTrigger.triggers.Add(entry);
                
                int capturedIndex = i;
                buttons[i].onClick.AddListener(() => {
                    if (inventory != null)
                    {
                        InventorySelectionManager.Instance.SelectSeed(capturedIndex);
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

        for (int i = 0; i < SeedInventory.Capacity && i < icons.Length; i++)
        {
            var seed = inventory.GetAt(i);
            var img = icons[i];
            if (!img) continue;

            img.sprite = seed ? seed.Icon : null;
            img.enabled = seed != null;
            
            img.color = (i == inventory.SelectedIndex) ? selectedTint :
                        (seed ? Color.white : unselectedTint);
            
            if (slotBackgrounds != null && i < slotBackgrounds.Length && slotBackgrounds[i] != null)
            {
                if (i == inventory.SelectedIndex && seed != null)
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

    private void DeleteSelectedSeed()
    {
        if (inventory == null) return;
        
        int selectedIndex = inventory.SelectedIndex;
        DeleteSeedAt(selectedIndex);
    }

    private void DeleteSeedAt(int index)
    {
        if (inventory == null) return;
        
        var seed = inventory.GetAt(index);
        if (seed != null)
        {
            inventory.RemoveAt(index);
        }
    }

    
    public void SetInventory(SeedInventory inv)
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
