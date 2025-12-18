using UnityEngine;

[CreateAssetMenu(menuName = "Items2/Effects/AttackDamageBonus2")]
public class AttackDamageBonusEffect2 : StatEffect2
{
    public int attackDamageAdditive = 5;

    public override void Apply(Player_Stats playerStats)
    {
        if (playerStats != null)
            playerStats.AttackDamageBonus += attackDamageAdditive;
    }

    public override void Remove(Player_Stats playerStats)
    {
        if (playerStats != null)
            playerStats.AttackDamageBonus -= attackDamageAdditive;
    }
}

