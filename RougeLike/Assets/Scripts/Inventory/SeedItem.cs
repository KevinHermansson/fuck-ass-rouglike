using UnityEngine;

[CreateAssetMenu(menuName = "Items/Seed")]
public class SeedItem : ItemBase
{
<<<<<<< Updated upstream
    public SeedEffectBase Effect;
=======
    [Header("Effect")]
    public StatEffect Effect;
>>>>>>> Stashed changes

    private void OnEnable()
    {
        Category = ItemCategory.Seed;
    }
}
