
using UnityEngine;
using TMPro;

[System.Serializable]
public class UpgradeType
{
    public string name;
    public float level = 1;
    public float cost = 5;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;

    public void Upgrade()
    {
        if (level >= 2)
        {
            cost = (int)(5 * Mathf.Pow(1.2f, level));
        }
        level++;
        Debug.Log(name + " upgraded");
    }

    public void UpdateUI()
    {
        levelText.text = "Level " + level.ToString();
        costText.text = cost.ToString() + "$";
    }
}
