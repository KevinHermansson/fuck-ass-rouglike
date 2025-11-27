using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bouncerBossPrefab;
    public GameObject flyerBossPrefab;
    public float spawnInterval = 5f;
    private float lastSpawnTime = 0f;
    private Camera mainCamera;
    private bool spawnBouncerNext = true;

    void Start()
    {
        mainCamera = Camera.main;
        lastSpawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time > lastSpawnTime + spawnInterval)
        {
            lastSpawnTime = Time.time;
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        GameObject prefabToSpawn;
        float yPos;

        if (spawnBouncerNext)
        {
            Debug.Log("Spawning Bouncer Boss");
            prefabToSpawn = bouncerBossPrefab;
            yPos = 3f;
        }
        else
        {
            Debug.Log("Spawning Flyer Boss");
            prefabToSpawn = flyerBossPrefab;
            yPos = -2f;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError(spawnBouncerNext ? "Bouncer boss prefab not assigned!" : "Flyer boss prefab not assigned!");
            return;
        }

        spawnBouncerNext = !spawnBouncerNext;

        Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(1.1f, 0.5f, 10f));
        spawnPosition.y = yPos;
        spawnPosition.z = 0;

        GameObject newBoss = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        if (newBoss == null)
        {
            Debug.LogError("Instantiate failed! The new boss object is null.");
        }
        else
        {
            Debug.Log("Successfully instantiated new boss: " + newBoss.name);
        }
    }
}
