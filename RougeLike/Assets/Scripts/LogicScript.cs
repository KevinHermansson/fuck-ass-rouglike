using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int PebbleCounter;
    public TextMeshProUGUI PebbleCounterText;
   
   public void UpdatePebbleCounter()
    {
       PebbleCounter = PebbleCounter + 1;
       PebbleCounterText.text = PebbleCounter.ToString();
   }

}
