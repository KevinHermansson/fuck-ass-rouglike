using UnityEngine;

public class PebbleManager : MonoBehaviour
{
    public static PebbleManager Instance;

    // Static variable to persist pebbles across death and scene changes
    private static int savedPebbles = 0;
    
    public int pebbles = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make PebbleManager persistent
            
            // Load saved pebbles immediately
            pebbles = savedPebbles;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Ensure pebbles are synced
        pebbles = savedPebbles;
        
        // Update UI with current pebble count
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (PebbleUI.Instance != null)
        {
            PebbleUI.Instance.UpdatePebbleText(pebbles);
        }
        else
        {
            // Try to find PebbleUI if instance not set
            PebbleUI ui = FindObjectOfType<PebbleUI>();
            if (ui != null)
            {
                ui.UpdatePebbleText(pebbles);
            }
        }
    }

    public void AddPebbles(int amount)
    {
        pebbles += amount;
        savedPebbles = pebbles; // Save to static variable immediately
        
        Debug.Log($"Pebbles added! Total: {pebbles}, Saved: {savedPebbles}");

        // Update UI and play bump animation
        UpdateUI();
        
        if (PebbleUI.Instance != null)
        {
            PebbleUI.Instance.PlayBump();
        }
        else
        {
            PebbleUI ui = FindObjectOfType<PebbleUI>();
            if (ui != null)
            {
                ui.PlayBump();
            }
        }
    }
    
    // Call this to reset pebbles (optional, for new game)
    public static void ResetPebbles()
    {
        savedPebbles = 0;
    }

    public void DropPebbles(GameObject prefabToDrop, int amount, Vector3 position)
    {
        if (prefabToDrop == null)
        {
            Debug.LogWarning("Prefab to drop is not assigned!");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            // Add a small random offset to the spawn position
            Vector3 spawnPosition = position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0f, 0.5f), 0);
            Instantiate(prefabToDrop, spawnPosition, Quaternion.identity);
        }
    }
}
