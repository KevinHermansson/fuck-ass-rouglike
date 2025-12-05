using UnityEngine;

// Base class - don't create this directly!
// Use SpeedBonusEffect2, MaxHealthBonusEffect2, etc. instead
public abstract class StatEffect2 : ScriptableObject
{
    [Header("Effect Info")]
    public string EffectName;
    
    public abstract void Apply(Player_Stats playerStats);
    public abstract void Remove(Player_Stats playerStats);
}
