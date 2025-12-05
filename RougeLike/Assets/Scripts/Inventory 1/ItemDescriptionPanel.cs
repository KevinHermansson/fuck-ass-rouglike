using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDescriptionPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Awake()
    {
        // Try to find components if not assigned
        if (iconImage == null)
        {
            iconImage = transform.Find("Icon")?.GetComponent<Image>();
        }
        
        if (descriptionText == null)
        {
            // Try both "ItemDescrition" (typo) and "ItemDescription"
            Transform descTransform = transform.Find("ItemDescrition");
            if (descTransform == null)
                descTransform = transform.Find("ItemDescription");
            
            descriptionText = descTransform?.GetComponent<TextMeshProUGUI>();
        }

        Clear();
    }

    public void ShowItem(ItemType2 item)
    {
        if (item == null)
        {
            Clear();
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = item.Icon;
            iconImage.enabled = item.Icon != null;
        }

        if (descriptionText != null)
        {
            descriptionText.text = string.IsNullOrEmpty(item.Description) 
                ? item.DisplayName 
                : item.Description;
        }
    }

    public void Clear()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (descriptionText != null)
        {
            descriptionText.text = "";
        }
    }
}

