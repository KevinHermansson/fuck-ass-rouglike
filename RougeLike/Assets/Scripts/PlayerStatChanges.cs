using UnityEngine;

public abstract class StatEffect : ScriptableObject
{
    public string EffectName;
    public abstract void Apply(Player_Stats playerStats);
    public abstract void Remove(Player_Stats playerStats);
}

[CreateAssetMenu(menuName = "Items/Effects/SpeedBonus")]
public class SpeedBonusEffect : StatEffect
{
    public float moveSpeedAdditive = 1f;

    public override void Apply(Player_Stats playerStats)
    {
        playerStats.SpeedBonus += moveSpeedAdditive;
    }

    public override void Remove(Player_Stats playerStats)
    {
        playerStats.SpeedBonus -= moveSpeedAdditive;
    }
}

[CreateAssetMenu(menuName = "Items/Effects/MaxHealthBonus")]
public class MaxHealthBonusEffect : StatEffect
{
    public int maxHealthAdd = 10;

    public override void Apply(Player_Stats playerStats)
    {
        playerStats.MaxHealthBonus += maxHealthAdd;
        playerStats.ClampHealth();
    }

    public override void Remove(Player_Stats playerStats)
    {
        playerStats.MaxHealthBonus -= maxHealthAdd;
        playerStats.ClampHealth();
    }
}
