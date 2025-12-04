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
    public int damageBonusPerLevel = 5;
    public float attackSpeedBonusPerLevel = 0.1f;
    public float jumpHeightBonusPerLevel = 0.5f;

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

        // Check if PebbleManager exists
        if (PebbleManager.Instance == null)
        {
            Debug.LogError("PebbleManager not found!");
            return;
        }

        switch (buttonTag)
        {
            case "Health":
                // Check if player has enough pebbles
                if (PebbleManager.Instance.pebbles < HealthCost)
                {
                    Debug.Log($"Not enough pebbles! Need {HealthCost}, have {PebbleManager.Instance.pebbles}");
                    return;
                }
                
                // Deduct cost
                PebbleManager.Instance.pebbles -= (int)HealthCost;
                
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
                if (PebbleManager.Instance.pebbles < DamageCost)
                {
                    Debug.Log($"Not enough pebbles! Need {DamageCost}, have {PebbleManager.Instance.pebbles}");
                    return;
                }
                
                PebbleManager.Instance.pebbles -= (int)DamageCost;
                
                if (DamageLevel >= 2)
                {
                    DamageCost = (int)(5 * Mathf.Pow(1.2f, DamageLevel));
                }
                DamageLevel++;
                if (playerStats != null)
                {
                    playerStats.AttackDamageBonus += damageBonusPerLevel;
                    Debug.Log($"Damage upgraded! AttackDamageBonus: {playerStats.AttackDamageBonus}, Total Damage: {playerStats.AttackDamage}");
                }
                else
                {
                    Debug.LogError("playerStats is null! Please assign Player_Stats in Inspector.");
                }
                break;
            case "AttackSpeed":
                if (PebbleManager.Instance.pebbles < AttackSpeedCost)
                {
                    Debug.Log($"Not enough pebbles! Need {AttackSpeedCost}, have {PebbleManager.Instance.pebbles}");
                    return;
                }
                
                PebbleManager.Instance.pebbles -= (int)AttackSpeedCost;
                
                if (AttackSpeedLevel >= 2)
                {
                    AttackSpeedCost = (int)(5 * Mathf.Pow(1.2f, AttackSpeedLevel));
                }
                AttackSpeedLevel++;
                if (playerStats != null)
                {
                    playerStats.AttackSpeedBonus += attackSpeedBonusPerLevel;
                    Debug.Log($"AttackSpeed upgraded! AttackSpeedBonus: {playerStats.AttackSpeedBonus}, Total AttackSpeed: {playerStats.AttackSpeed}");
                }
                else
                {
                    Debug.LogError("playerStats is null! Please assign Player_Stats in Inspector.");
                }
                break;
            case "Speed":
                if (PebbleManager.Instance.pebbles < SpeedCost)
                {
                    Debug.Log($"Not enough pebbles! Need {SpeedCost}, have {PebbleManager.Instance.pebbles}");
                    return;
                }
                
                PebbleManager.Instance.pebbles -= (int)SpeedCost;
                
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
                if (PebbleManager.Instance.pebbles < JumpHeightCost)
                {
                    Debug.Log($"Not enough pebbles! Need {JumpHeightCost}, have {PebbleManager.Instance.pebbles}");
                    return;
                }
                
                PebbleManager.Instance.pebbles -= (int)JumpHeightCost;
                
                if (JumpHeightLevel >= 2)
                {
                    JumpHeightCost = (int)(5 * Mathf.Pow(1.2f, JumpHeightLevel));
                }
                JumpHeightLevel++;
                if (playerStats != null)
                {
                    playerStats.JumpHeightBonus += jumpHeightBonusPerLevel;
                    Debug.Log($"JumpHeight upgraded! JumpHeightBonus: {playerStats.JumpHeightBonus}, Total JumpHeight: {playerStats.JumpHeight}");
                }
                else
                {
                    Debug.LogError("playerStats is null! Please assign Player_Stats in Inspector.");
                }
                break;
        }
        
        // Update UI after purchase
        if (PebbleUI.Instance != null)
        {
            PebbleUI.Instance.UpdatePebbleText(PebbleManager.Instance.pebbles);
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
