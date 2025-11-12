using UnityEngine;

public enum ItemCategory { Regular, Seed }

public abstract class ItemBase : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public Sprite Icon;
    public ItemCategory Category;
}
