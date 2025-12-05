using UnityEngine;
using TMPro;

public class BossSpawner : MonoBehaviour
{
    public GameObject bouncerBossPrefab;
    public GameObject flyerBossPrefab;
    public TextMeshProUGUI bossHPText; // Drag the BossHP text here
    public float spawnInterval = 4f;
    public float flyerSpawnInterval = 2f; // Shorter interval for flyer bosses
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
        // Check if player is at x position 422 or higher
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null || playerObj.transform.position.x < 422f)
        {
            return;
        }
        
        // Get current boss HP from the canvas text
        int bossHP = GetBossHP();
        
        // Stop spawning if boss HP is 0 or less
        if (bossHP <= 0)
        {
            Debug.Log("BossSpawner: Boss HP is 0, stopping spawns");
            return;
        }
        
        Debug.Log($"BossSpawner: Boss HP = {bossHP}, checking spawn timer");
        
        float currentInterval = spawnBouncerNext ? spawnInterval : flyerSpawnInterval;
        
        // If BossHP < 7, make flyers spawn faster
        if (bossHP < 7 && !spawnBouncerNext)
        {
            currentInterval = flyerSpawnInterval * 0.5f; // 50% faster
        }
        
        // If BossHP < 4, make both bosses spawn more often
        if (bossHP < 4)
        {
            currentInterval = currentInterval * 0.4f; // 40% faster for both
        }
        
        Debug.Log($"BossSpawner: Time check - Current: {Time.time}, Last: {lastSpawnTime}, Interval: {currentInterval}, Ready: {Time.time > lastSpawnTime + currentInterval}");
        
        if (Time.time > lastSpawnTime + currentInterval)
        {
            lastSpawnTime = Time.time;
            Debug.Log("BossSpawner: TIME TO SPAWN! Calling SpawnBoss()");
            SpawnBoss();
        }
    }
    
    int GetBossHP()
    {
        // Try to read from the text component first
        if (bossHPText != null && !string.IsNullOrEmpty(bossHPText.text))
        {
            if (int.TryParse(bossHPText.text, out int hp))
            {
                return hp;
            }
        }
        
        // Fallback: Find the BossHeart component in the scene
        BossHeart bossHeart = FindObjectOfType<BossHeart>();
        if (bossHeart != null)
        {
            return bossHeart.GetCurrentHeartCount();
        }
        
        return 10; // Default value if nothing found
    }

    void SpawnBoss()
    {
        GameObject prefabToSpawn;
        float yPos;

        if (spawnBouncerNext)
        {
            Debug.Log("Spawning Bouncer Boss");
            prefabToSpawn = bouncerBossPrefab;
            yPos = 96f;
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
                    yPos = Mathf.Max(playerObj.transform.position.y, 91f);
                    Debug.Log("Flyer boss spawning at player Y level: " + yPos);
                }
                else
                {
                    yPos = 91f;
                }
            }
            else
            {
                yPos = 91f;
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
            // Get current boss HP to determine speed multiplier
            int bossHP = GetBossHP();
            float speedMultiplier = 1f;
            
            if (bossHP < 7)
            {
                speedMultiplier = 1.5f; // 50% faster
            }
            if (bossHP < 4)
            {
                speedMultiplier = 2f; // 100% faster (2x speed)
            }
            
            // Apply speed multiplier to the spawned boss
            BossSpawnMovement bouncerMovement = newBoss.GetComponent<BossSpawnMovement>();
            if (bouncerMovement != null)
            {
                bouncerMovement.speed *= speedMultiplier;
            }
            
            FlyerBoss flyerMovement = newBoss.GetComponent<FlyerBoss>();
            if (flyerMovement != null)
            {
                flyerMovement.speed *= speedMultiplier;
            }
            
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
            
            Debug.Log("Successfully instantiated new boss: " + newBoss.name + " with speed multiplier: " + speedMultiplier);
        }
    }
}
