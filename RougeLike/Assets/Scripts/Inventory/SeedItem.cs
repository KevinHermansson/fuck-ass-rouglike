using UnityEngine;

[CreateAssetMenu(menuName = "Items/Seed")]
public class SeedItem : ItemBase
{
    public StatEffect Effect;

    private void OnEnable()
    {
        Category = ItemCategory.Seed;
    }
}
