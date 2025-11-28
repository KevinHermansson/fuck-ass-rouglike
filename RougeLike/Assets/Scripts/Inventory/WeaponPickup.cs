using UnityEngine;

/// <summary>
/// Specialized pickup for weapons. Can be used instead of RegularItemPickup for weapons.
/// This is essentially a wrapper that ensures the item is a WeaponItem.
/// </summary>
public class WeaponPickup : RegularItemPickup
{
    [Header("Weapon Settings")]
    [Tooltip("The weapon to give. This should be a WeaponItem ScriptableObject.")]
    public WeaponItem weaponToGive;
    
    private void OnValidate()
    {
        // Automatically set itemToGive to weaponToGive if weaponToGive is set
        if (weaponToGive != null && itemToGive != weaponToGive)
        {
            itemToGive = weaponToGive;
        }
    }
    
    private void Start()
    {
        // Ensure itemToGive is set from weaponToGive
        if (weaponToGive != null && itemToGive != weaponToGive)
        {
            itemToGive = weaponToGive;
        }
        
        // Validate that we have a weapon
        if (itemToGive != null && !(itemToGive is WeaponItem))
        {
            Debug.LogWarning($"WeaponPickup on {gameObject.name} has a RegularItem that is not a WeaponItem. This may cause issues.");
        }
    }
}

