using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bouncerBossPrefab;
    public GameObject flyerBossPrefab;
    public float spawnInterval = 5f;
    public float flyerSpawnInterval = 3f; // Shorter interval for flyer bosses
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
        float currentInterval = spawnBouncerNext ? spawnInterval : flyerSpawnInterval;
        if (Time.time > lastSpawnTime + currentInterval)
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
            
            // 25% chance to spawn at player's Y level, otherwise at default height
            if (Random.Range(0, 4) == 0)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    yPos = playerObj.transform.position.y;
                    Debug.Log("Flyer boss spawning at player Y level: " + yPos);
                }
                else
                {
                    yPos = -2f;
                }
            }
            else
            {
                yPos = -2f;
            }
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError(spawnBouncerNext ? "Bouncer boss prefab not assigned!" : "Flyer boss prefab not assigned!");
            return;
        }

        spawnBouncerNext = !spawnBouncerNext;

        // Randomly choose left or right side
        bool spawnFromRight = Random.Range(0, 2) == 0;
        float xViewport = spawnFromRight ? 1.1f : -0.1f; // Right side or left side of camera
        
        Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(xViewport, 0.5f, 10f));
        spawnPosition.y = yPos;
        spawnPosition.z = 0;

        GameObject newBoss = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        if (newBoss == null)
        {
            Debug.LogError("Instantiate failed! The new boss object is null.");
        }
        else
        {
            // Flip both boss types if they spawn to the right of the player
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                bool bossIsRightOfPlayer = newBoss.transform.position.x > playerObj.transform.position.x;
                if (bossIsRightOfPlayer)
                {
                    newBoss.transform.localScale = new Vector3(-newBoss.transform.localScale.x, newBoss.transform.localScale.y, newBoss.transform.localScale.z);
                }
            }
            
            Debug.Log("Successfully instantiated new boss: " + newBoss.name);
        }
    }
}
