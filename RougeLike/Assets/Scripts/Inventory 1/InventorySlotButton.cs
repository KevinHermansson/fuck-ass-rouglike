using UnityEngine;
using UnityEngine.UI;

public class InventorySlotButton : MonoBehaviour
{
    [Header("Slot Info")]
    [SerializeField] private int slotIndex;
    [SerializeField] private ItemCategory inventoryType; // Seed, Item, or Weapon

    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image plantImage; // For seed slots - shows when seed is present
    [SerializeField] private Button button;

    private InventoryUI2 parentUI;

    public int SlotIndex => slotIndex;
    public ItemCategory InventoryType => inventoryType;

    public void Initialize(int index, ItemCategory type)
    {
        slotIndex = index;
        inventoryType = type;
    }

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (iconImage == null)
        {
            // Try to find an Image component in children (but not the button's own Image)
            Image[] images = GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                if (img.gameObject != gameObject) // Not the button's own image
                {
                    iconImage = img;
                    break;
                }
            }
            
            // If still not found, try finding a child named "Icon"
            if (iconImage == null)
            {
                Transform iconTransform = transform.Find("Icon");
                if (iconTransform != null)
                {
                    iconImage = iconTransform.GetComponent<Image>();
                }
            }
        }

        // Auto-find plant image for seed slots
        if (plantImage == null)
        {
            Transform plantTransform = transform.Find("Plant");
            if (plantTransform != null)
            {
                plantImage = plantTransform.GetComponent<Image>();
            }
        }

        parentUI = GetComponentInParent<InventoryUI2>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    public void OnButtonClicked()
    {
        if (parentUI != null)
        {
            parentUI.OnSlotClicked(slotIndex);
        }
    }

    public void UpdateSlot(ItemType2 item)
    {
        // Update icon image
        if (iconImage != null)
        {
            Sprite spriteToShow = null;
            
            if (item != null)
            {
                // For weapons, prefer weaponData.weaponSprite if Icon is not set
                if (item.Category == ItemCategory.Weapon && item.weaponData != null && item.weaponData.weaponSprite != null)
                {
                    spriteToShow = item.weaponData.weaponSprite;
                }
                else if (item.Icon != null)
                {
                    spriteToShow = item.Icon;
                }
            }
            
            if (spriteToShow != null)
            {
                iconImage.sprite = spriteToShow;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }
        }
        
        // Update plant image for seed slots - show when seed is present, hide when empty
        if (plantImage != null && inventoryType == ItemCategory.Seed)
        {
            plantImage.gameObject.SetActive(item != null);
        }
    }

    public void SetSelected(bool selected)
    {
        // You can add visual feedback here (highlight, border, etc.)
        // For now, just a placeholder
    }
}

