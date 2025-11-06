using UnityEngine;

public class PebbleManager : MonoBehaviour
{
    public static PebbleManager Instance;

    public int pebbles = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddPebbles(int amount)
    {
        pebbles += amount;

        if (PebbleUI.Instance != null)
        {
            PebbleUI.Instance.UpdatePebbleText(pebbles);
            PebbleUI.Instance.PlayBump();
        }
    }
}
