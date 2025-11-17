using UnityEngine;

public abstract class SeedEffectBase : ScriptableObject
{
    public string EffectName;
    public abstract void Apply(PlayerStats stats);
    public abstract void Remove(PlayerStats stats);
}
