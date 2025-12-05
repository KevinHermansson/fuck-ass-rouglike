using UnityEngine;

public enum WeaponType { Gun, Melee }

[CreateAssetMenu(menuName = "Items2/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Type")]
    public WeaponType weaponType = WeaponType.Gun;

    [Header("Gun Properties")]
    public GameObject bulletPrefab;
    public int bulletCount = 1; // Number of bullets per shot
    public float bulletSpread = 0f; // Spread angle in degrees
    public float bulletSpeed = 10f;
    public float bulletLifetime = 5f;
    public float fireRate = 1f; // Shots per second

    [Header("Melee Properties")]
    public float meleeRange = 0.5f;
    public float meleeDamageMultiplier = 1f; // Multiplies base attack damage
    public float meleeSwingArc = 90f; // Arc in degrees (for area attacks)

    [Header("General Properties")]
    public int baseDamage = 20; // Base damage (gets multiplied by player stats)
    public float cooldownMultiplier = 1f; // Affects attack speed

    [Header("Visuals")]
    public Sprite weaponSprite; // Sprite to show when equipped
    public Vector2 weaponOffset = new Vector2(0.5f, 0f); // Offset from player
    public float weaponScale = 1f; // Scale multiplier for weapon sprite
    public GameObject slashEffectPrefab; // For melee weapons (white slash effect)
}

