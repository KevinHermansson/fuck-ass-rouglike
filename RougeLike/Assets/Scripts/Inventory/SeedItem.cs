using UnityEngine;

[CreateAssetMenu(menuName = "Items/Seed")]
public class SeedItem : ItemBase
{
    public SeedEffectBase Effect;

    private void OnEnable()
    {
        Category = ItemCategory.Seed;
    }
}
