using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private GameObject weaponInfoPanel;
    
    [Header("Settings")]
    [SerializeField] private string ammoFormat = "{0} / {1}";
    [SerializeField] private string infiniteAmmoText = "∞";
    
    private WeaponController weaponController;
    
    private void Start()
    {
        // Find weapon controller
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            weaponController = player.GetComponent<WeaponController>();
            if (weaponController == null)
            {
                Debug.LogWarning("WeaponController not found on player. Weapon HUD will not update.");
                return;
            }
        }
        
        // Subscribe to weapon events
        if (weaponController != null)
        {
            weaponController.OnWeaponChanged += OnWeaponChanged;
            weaponController.OnAmmoChanged += OnAmmoChanged;
        }
        
        // Hide weapon info initially
        if (weaponInfoPanel != null)
        {
            weaponInfoPanel.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        if (weaponController != null)
        {
            weaponController.OnWeaponChanged -= OnWeaponChanged;
            weaponController.OnAmmoChanged -= OnAmmoChanged;
        }
    }
    
    private void OnWeaponChanged(WeaponItem weapon, int ammo)
    {
        if (weapon == null)
        {
            // No weapon equipped
            if (weaponInfoPanel != null)
            {
                weaponInfoPanel.SetActive(false);
            }
            return;
        }
        
        // Show weapon info
        if (weaponInfoPanel != null)
        {
            weaponInfoPanel.SetActive(true);
        }
        
        // Update weapon icon
        if (weaponIcon != null)
        {
            Sprite iconToUse = weapon.weaponSprite != null ? weapon.weaponSprite : weapon.Icon;
            weaponIcon.sprite = iconToUse;
            weaponIcon.enabled = iconToUse != null;
        }
        
        // Update weapon name
        if (weaponNameText != null)
        {
            weaponNameText.text = weapon.DisplayName;
        }
        
        // Update ammo display
        UpdateAmmoDisplay(ammo, weapon.maxAmmo, weapon.infiniteAmmo);
    }
    
    private void OnAmmoChanged(int current, int max)
    {
        if (weaponController != null)
        {
            WeaponItem weapon = weaponController.GetCurrentWeapon();
            if (weapon != null)
            {
                UpdateAmmoDisplay(current, max, weapon.infiniteAmmo);
            }
        }
    }
    
    private void UpdateAmmoDisplay(int current, int max, bool infinite)
    {
        if (ammoText == null) return;
        
        if (infinite)
        {
            ammoText.text = infiniteAmmoText;
        }
        else
        {
            ammoText.text = string.Format(ammoFormat, current, max);
        }
    }
}

