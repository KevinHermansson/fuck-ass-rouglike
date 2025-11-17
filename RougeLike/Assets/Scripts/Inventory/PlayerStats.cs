using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float BaseMoveSpeed = 5f;
    public int BaseMaxHealth = 100;

    public float SpeedBonus { get; set; }
    public int MaxHealthBonus { get; set; }

    public float MoveSpeed => BaseMoveSpeed + SpeedBonus;
    public int MaxHealth   => BaseMaxHealth + MaxHealthBonus;

    public int CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void ClampHealth()
    {
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
    }
}
