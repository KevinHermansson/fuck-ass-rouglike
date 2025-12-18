using UnityEngine;

[CreateAssetMenu(menuName = "Items2/Effects/JumpHeightBonus2")]
public class JumpHeightBonusEffect2 : StatEffect2
{
    public float jumpHeightAdditive = 2f;

    public override void Apply(Player_Stats playerStats)
    {
        if (playerStats != null)
            playerStats.JumpHeightBonus += jumpHeightAdditive;
    }

    public override void Remove(Player_Stats playerStats)
    {
        if (playerStats != null)
            playerStats.JumpHeightBonus -= jumpHeightAdditive;
    }
}

