using UnityEngine;

[CreateAssetMenu(menuName = "Items2/Effects/SpeedBonus2")]
public class SpeedBonusEffect2 : StatEffect2
{
    public float moveSpeedAdditive = 1f;

    public override void Apply(Player_Stats playerStats)
    {
        if (playerStats != null)
            playerStats.SpeedBonus += moveSpeedAdditive;
    }

    public override void Remove(Player_Stats playerStats)
    {
        if (playerStats != null)
            playerStats.SpeedBonus -= moveSpeedAdditive;
    }
}

