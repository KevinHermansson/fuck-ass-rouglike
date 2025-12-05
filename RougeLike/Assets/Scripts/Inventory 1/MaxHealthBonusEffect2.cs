using UnityEngine;

[CreateAssetMenu(menuName = "Items2/Effects/MaxHealthBonus2")]
public class MaxHealthBonusEffect2 : StatEffect2
{
    public int maxHealthAdd = 10;

    public override void Apply(Player_Stats playerStats)
    {
        if (playerStats != null)
        {
            playerStats.MaxHealthBonus += maxHealthAdd;
            playerStats.ClampHealth();
        }
    }

    public override void Remove(Player_Stats playerStats)
    {
        if (playerStats != null)
        {
            playerStats.MaxHealthBonus -= maxHealthAdd;
            playerStats.ClampHealth();
        }
    }
}

