using UnityEngine;

[CreateAssetMenu(menuName = "Items/Regular")]
public class RegularItem : ItemBase
{
    private void OnEnable()
    {
        Category = ItemCategory.Regular;
    }
}

