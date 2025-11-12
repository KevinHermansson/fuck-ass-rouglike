using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/SpeedBonus")]
public class SpeedBonusEffect : SeedEffectBase
{
    public float moveSpeedAdditive = 1f;

    public override void Apply(PlayerStats stats)  { stats.SpeedBonus += moveSpeedAdditive; }
    public override void Remove(PlayerStats stats) { stats.SpeedBonus -= moveSpeedAdditive; }
}

[CreateAssetMenu(menuName = "Items/Effects/MaxHealthBonus")]
public class MaxHealthBonusEffect : SeedEffectBase
{
    public int maxHealthAdd = 10;

    public override void Apply(PlayerStats stats)  { stats.MaxHealthBonus += maxHealthAdd; stats.ClampHealth(); }
    public override void Remove(PlayerStats stats) { stats.MaxHealthBonus -= maxHealthAdd; stats.ClampHealth(); }
}
