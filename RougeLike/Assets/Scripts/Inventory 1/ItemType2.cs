using UnityEngine;

public enum ItemCategory { Seed, Item, Weapon }

[CreateAssetMenu(menuName = "Items2/ItemType2")]
public class ItemType2 : ScriptableObject
{
    [Header("Basic Info")]
    public string Id;
    public string DisplayName;
    public Sprite Icon;
    public ItemCategory Category;
    [TextArea(3, 5)]
    public string Description;
    
    [Header("Effects")]
    public StatEffect2 Effect2;
    
    [Header("Weapon Data (if Category is Weapon)")]
    public WeaponData weaponData;
}