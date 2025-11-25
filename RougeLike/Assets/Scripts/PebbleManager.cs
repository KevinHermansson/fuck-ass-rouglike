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
