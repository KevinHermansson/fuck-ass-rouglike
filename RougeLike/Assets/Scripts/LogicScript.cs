using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicScript : MonoBehaviour
{
    // Static variable to persist across scene loads and death
    private static int savedPebbleCounter = 0;
    
    public int PebbleCounter;
    public TextMeshProUGUI PebbleCounterText;
   
    void Start()
    {
        // Load the saved pebble count
        PebbleCounter = savedPebbleCounter;
        UpdateDisplay();
    }
   
    public void UpdatePebbleCounter()
    {
        PebbleCounter = PebbleCounter + 1;
        savedPebbleCounter = PebbleCounter; // Save to static variable
        UpdateDisplay();
    }
    
    void UpdateDisplay()
    {
        if (PebbleCounterText != null)
        {
            PebbleCounterText.text = PebbleCounter.ToString();
        }
    }
    
    // Call this to reset pebbles (if needed for new game)
    public static void ResetPebbles()
    {
        savedPebbleCounter = 0;
    }
}
