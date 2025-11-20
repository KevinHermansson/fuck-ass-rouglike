using UnityEngine;
using UnityEngine.EventSystems;

public class Upgrades : MonoBehaviour
{
    public TMPro.TextMeshProUGUI HealthLevelText;
    public TMPro.TextMeshProUGUI DamageLevelText;
    public TMPro.TextMeshProUGUI AttackSpeedLevelText;
    public TMPro.TextMeshProUGUI SpeedLevelText;
    public TMPro.TextMeshProUGUI JumpHeightLevelText;
    public TMPro.TextMeshProUGUI HealthCostText;
    public TMPro.TextMeshProUGUI DamageCostText;
    public TMPro.TextMeshProUGUI AttackSpeedCostText;
    public TMPro.TextMeshProUGUI SpeedCostText;
    public TMPro.TextMeshProUGUI JumpHeightCostText;

    public Player_Stats playerStats;
    public int healthBonusPerLevel = 10;
    public float speedBonusPerLevel = 0.5f;

    public float HealthLevel = 1;
    public float HealthCost = 5;
    public float DamageLevel = 1;
    public float DamageCost = 5;
    public float AttackSpeedLevel = 1;
    public float AttackSpeedCost = 5;
    public float SpeedLevel = 1;
    public float SpeedCost = 5;
    public float JumpHeightLevel = 1;
    public float JumpHeightCost = 5;

    private void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<Player_Stats>();
            if (playerStats == null)
            {
                Debug.LogError("Could not find Player_Stats component in the scene!");
            }
        }
    }

    private void Update()
    {
        UpdateLevelTexts();
        UpdateCostTexts();
    }
    public void Upgrade()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            Debug.LogError("No button selected!");
            return;
        }

        string buttonTag = EventSystem.current.currentSelectedGameObject.tag;
        Debug.Log($"Upgrade button clicked with tag: {buttonTag}");


        switch (buttonTag)
        {
            case "Health":
                if (HealthLevel >= 2)
                {
                    HealthCost = (int)(5 * Mathf.Pow(1.2f, HealthLevel));
                }
                HealthLevel++;
                if (playerStats != null)
                {
                    playerStats.MaxHealthBonus += healthBonusPerLevel;
                    playerStats.health = playerStats.MaxHealth; // Heal to new max
                    Debug.Log($"Health upgraded! MaxHealthBonus: {playerStats.MaxHealthBonus}, MaxHealth: {playerStats.MaxHealth}, Current Health: {playerStats.health}");
                }
                else
                {
                    Debug.LogError("playerStats is null! Please assign Player_Stats in Inspector.");
                }
                break;
            case "Damage":
                if (DamageLevel >= 2)
                {
                    DamageCost = (int)(5 * Mathf.Pow(1.2f, DamageLevel));
                }
                DamageLevel++;
                Debug.Log("Damage upgraded");
                break;
            case "AttackSpeed":
                if (AttackSpeedLevel >= 2)
                {
                    AttackSpeedCost = (int)(5 * Mathf.Pow(1.2f, AttackSpeedLevel));
                }
                AttackSpeedLevel++;
                Debug.Log("AttackSpeed upgraded");
                break;
            case "Speed":
                if (SpeedLevel >= 2)
                {
                    SpeedCost = (int)(5 * Mathf.Pow(1.2f, SpeedLevel));
                }
                SpeedLevel++;
                if (playerStats != null)
                {
                    playerStats.SpeedBonus += speedBonusPerLevel;
                }
                Debug.Log("Speed upgraded");
                break;
            case "JumpHeight":
                if (JumpHeightLevel >= 2)
                {
                    JumpHeightCost = (int)(5 * Mathf.Pow(1.2f, JumpHeightLevel));
                }
                JumpHeightLevel++;
                Debug.Log("JumpHeight upgraded");
                break;
        }
    }

    void UpdateCostTexts()
    {
        HealthCostText.text = HealthCost.ToString() + "$";
        DamageCostText.text = DamageCost.ToString() + "$";
        AttackSpeedCostText.text = AttackSpeedCost.ToString() + "$";
        SpeedCostText.text = SpeedCost.ToString() + "$";
        JumpHeightCostText.text = JumpHeightCost.ToString() + "$";
    }

    void UpdateLevelTexts()
    {
        HealthLevelText.text = "Level " + HealthLevel.ToString();
        DamageLevelText.text = "Level " + DamageLevel.ToString();
        AttackSpeedLevelText.text = "Level " + AttackSpeedLevel.ToString();
        SpeedLevelText.text = "Level " + SpeedLevel.ToString();
        JumpHeightLevelText.text = "Level " + JumpHeightLevel.ToString();
    }
}
