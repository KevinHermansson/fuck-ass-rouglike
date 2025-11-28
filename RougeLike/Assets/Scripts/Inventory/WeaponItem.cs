using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponItem : RegularItem
{
    [Header("Weapon Stats")]
    [Tooltip("Damage per shot")]
    public int damage = 25;
    
    [Tooltip("Fire rate in shots per second")]
    public float fireRate = 2f;
    
    [Tooltip("Maximum ammo capacity")]
    public int maxAmmo = 30;
    
    [Tooltip("Starting ammo when weapon is picked up")]
    public int startingAmmo = 30;
    
    [Tooltip("Projectile prefab to spawn when shooting")]
    public GameObject projectilePrefab;
    
    [Tooltip("Speed of the projectile")]
    public float projectileSpeed = 15f;
    
    [Tooltip("Lifetime of projectile in seconds")]
    public float projectileLifetime = 3f;
    
    [Tooltip("Range of the weapon (for raycast-based weapons)")]
    public float range = 10f;
    
    [Tooltip("Does this weapon use ammo?")]
    public bool usesAmmo = true;
    
    [Tooltip("Does this weapon have infinite ammo?")]
    public bool infiniteAmmo = false;
    
    [Header("Visual")]
    [Tooltip("Sprite to show when weapon is equipped in HUD")]
    public Sprite weaponSprite;
    
    private void OnEnable()
    {
        Category = ItemCategory.Regular;
    }
}

