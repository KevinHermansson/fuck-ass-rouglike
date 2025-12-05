using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    
    [Header("Debug")]
    [SerializeField] private bool showMeleeCone = false;
    
    private Player_Stats playerStats;
    private float lastDirection = 1f;
    private float nextAttackTime = 0f;
    private WeaponData currentWeapon;
    private int currentWeaponSlot = -1;
    private SpriteRenderer weaponSpriteRenderer;
    private GameObject weaponVisual;
    private bool isSwinging = false; // Flag to prevent UpdateWeaponVisual from interfering with swing animation

    private void Start()
{
    playerStats = GetComponent<Player_Stats>();
    
    // Try to find attack point from PlayerAttack component
    if (attackPoint == null)
    {
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            attackPoint = playerAttack.attackPoint;
        }
    }
    
    // Try to find attack point as child
    if (attackPoint == null)
    {
        Transform found = transform.Find("attackPoint");
        if (found == null)
            found = transform.Find("AttackPoint");
        if (found != null)
            attackPoint = found;
    }
    
    // Auto-create attack point if still not found
    if (attackPoint == null)
    {
        GameObject attackPointObj = new GameObject("attackPoint");
        attackPointObj.transform.SetParent(transform);
        attackPointObj.transform.localPosition = new Vector3(0.5f, 0f, 0f);
        attackPoint = attackPointObj.transform;
    }
    
    // Get enemy layers from PlayerAttack if not assigned
    if (enemyLayers.value == 0)
    {
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            enemyLayers = playerAttack.enemyLayers;
        }
    }
    
    // Create weapon visual GameObject
    CreateWeaponVisual();
    
    // Subscribe to weapon inventory changes
    if (WeaponInventory2.Instance != null)
    {
        WeaponInventory2.Instance.OnChanged += UpdateEquippedWeapon;
        WeaponInventory2.Instance.OnSelectionChanged += OnWeaponSelectionChanged;
    }
    
    UpdateEquippedWeapon();
}

    private void OnDestroy()
    {
        if (WeaponInventory2.Instance != null)
        {
            WeaponInventory2.Instance.OnChanged -= UpdateEquippedWeapon;
            WeaponInventory2.Instance.OnSelectionChanged -= OnWeaponSelectionChanged;
        }
    }

    private void CreateWeaponVisual()
    {
        weaponVisual = new GameObject("WeaponVisual");
        weaponVisual.transform.SetParent(transform);
        weaponVisual.transform.localPosition = Vector3.zero;
        
        weaponSpriteRenderer = weaponVisual.AddComponent<SpriteRenderer>();
        weaponSpriteRenderer.sortingOrder = 5; // Above player, below UI
        weaponVisual.SetActive(false);
    }

    private void Update()
    {
        // Track direction
        if (Input.GetKey(KeyCode.D))
            lastDirection = 1f;
        else if (Input.GetKey(KeyCode.A))
            lastDirection = -1f;

        // Update weapon visual position and rotation
        UpdateWeaponVisual();

        // Attack input
        if (Input.GetKeyDown(KeyCode.K) && Time.time >= nextAttackTime)
        {
            if (currentWeapon != null)
            {
                if (currentWeapon.weaponType == WeaponType.Melee)
                {
                    PerformMeleeAttack();
                }
                else if (currentWeapon.weaponType == WeaponType.Gun)
                {
                    PerformGunAttack();
                }
            }
            else
            {
                // No weapon equipped - use default melee
                PerformDefaultMelee();
            }
        }
    }

    private void UpdateWeaponVisual()
    {
        if (weaponVisual == null || weaponSpriteRenderer == null) return;
        
        // Don't update during swing animation
        if (isSwinging) return;

        if (currentWeapon != null && currentWeapon.weaponSprite != null)
        {
            weaponVisual.SetActive(true);
            weaponSpriteRenderer.sprite = currentWeapon.weaponSprite;
            
            // Apply weapon scale
            float scale = currentWeapon.weaponScale > 0 ? currentWeapon.weaponScale : 1f;
            weaponVisual.transform.localScale = new Vector3(scale, scale, 1f);
            
            // Position weapon in front of player
            Vector2 offset = currentWeapon.weaponOffset;
            offset.x *= lastDirection; // Flip offset based on direction
            weaponVisual.transform.localPosition = new Vector3(offset.x, offset.y, 0f);
            
            // Reset rotation
            weaponVisual.transform.localEulerAngles = Vector3.zero;
            
            // Flip sprite if facing left
            weaponSpriteRenderer.flipX = lastDirection < 0;
        }
        else
        {
            weaponVisual.SetActive(false);
        }
    }

    private void UpdateEquippedWeapon()
    {
        if (WeaponInventory2.Instance == null) return;

        // Get weapon from selected slot
        if (WeaponInventory2.Instance.SelectedIndex >= 0)
        {
            ItemType2 weaponItem = WeaponInventory2.Instance.GetAt(WeaponInventory2.Instance.SelectedIndex);
            if (weaponItem != null && weaponItem.weaponData != null)
            {
                currentWeapon = weaponItem.weaponData;
                currentWeaponSlot = WeaponInventory2.Instance.SelectedIndex;
                return;
            }
        }

        // No weapon selected, try to use first available weapon
        for (int i = 0; i < WeaponInventory2.Capacity; i++)
        {
            ItemType2 weaponItem = WeaponInventory2.Instance.GetAt(i);
            if (weaponItem != null && weaponItem.weaponData != null)
            {
                currentWeapon = weaponItem.weaponData;
                currentWeaponSlot = i;
                WeaponInventory2.Instance.SelectedIndex = i;
                return;
            }
        }

        // No weapon found
        currentWeapon = null;
        currentWeaponSlot = -1;
    }

    private void OnWeaponSelectionChanged(int selectedIndex)
    {
        UpdateEquippedWeapon();
    }

    private void PerformGunAttack()
    {
        if (currentWeapon == null || currentWeapon.bulletPrefab == null) return;

        float baseCooldown = 1f / currentWeapon.fireRate;
        float cooldown = baseCooldown * currentWeapon.cooldownMultiplier;
        
        if (playerStats != null)
        {
            cooldown /= playerStats.AttackSpeed;
        }

        // Fire bullets
        for (int i = 0; i < currentWeapon.bulletCount; i++)
        {
            float spreadAngle = 0f;
            
            // Calculate spread for each bullet
            if (currentWeapon.bulletCount > 1)
            {
                float spreadStep = currentWeapon.bulletSpread / (currentWeapon.bulletCount - 1);
                spreadAngle = -currentWeapon.bulletSpread / 2f + (spreadStep * i);
            }

            FireBullet(spreadAngle);
        }

        nextAttackTime = Time.time + cooldown;
    }

    private void FireBullet(float spreadAngle)
    {
        // Spawn bullet from weapon tip (or attack point)
        Vector3 spawnPos;
        if (weaponVisual != null && weaponSpriteRenderer != null && weaponSpriteRenderer.sprite != null)
        {
            // Calculate weapon tip position
            float weaponWidth = weaponSpriteRenderer.bounds.size.x;
            spawnPos = weaponVisual.transform.position + Vector3.right * (weaponWidth * 0.5f * lastDirection);
        }
        else
        {
            spawnPos = attackPoint != null ? attackPoint.position : transform.position;
        }
        
        GameObject bullet = Instantiate(currentWeapon.bulletPrefab, spawnPos, Quaternion.identity);

        // Calculate direction with spread
        Vector2 direction = new Vector2(lastDirection, 0f);
        float angleRad = spreadAngle * Mathf.Deg2Rad;
        direction = new Vector2(
            direction.x * Mathf.Cos(angleRad) - direction.y * Mathf.Sin(angleRad),
            direction.x * Mathf.Sin(angleRad) + direction.y * Mathf.Cos(angleRad)
        );

        // Apply velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * currentWeapon.bulletSpeed;
        }

        // Set up bullet damage and lifetime
        StartCoroutine(HandleBullet(bullet));
    }

    private IEnumerator HandleBullet(GameObject bullet)
    {
        float elapsed = 0f;
        float checkRadius = 0.2f;
        int damage = CalculateDamage();

        while (bullet != null && elapsed < currentWeapon.bulletLifetime)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(bullet.transform.position, checkRadius, enemyLayers);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    ApplyDamage(hits[i].gameObject, damage);
                }
                Destroy(bullet);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (bullet != null)
            Destroy(bullet);
    }

    private void PerformMeleeAttack()
    {
        if (currentWeapon == null) return;

        float baseCooldown = 1f / (playerStats != null ? playerStats.AttackSpeed : 1f);
        float cooldown = baseCooldown * currentWeapon.cooldownMultiplier;
        nextAttackTime = Time.time + cooldown;

        // Animate weapon swing
        StartCoroutine(AnimateWeaponSwing());

        // Spawn melee cone (similar to bullets)
        SpawnMeleeCone();
    }

    private void SpawnMeleeCone()
{
    // Use attackPoint as the origin of the melee
    Transform origin = attackPoint != null ? attackPoint : transform;

    GameObject cone = new GameObject("MeleeCone");

    // Parent to the origin so it moves with the player/weapon
    cone.transform.SetParent(origin);
    cone.transform.localPosition = Vector3.zero;
    cone.transform.localRotation = Quaternion.identity;

    // Visual debug cone
    CreateConeVisual(cone, currentWeapon.meleeRange, currentWeapon.meleeSwingArc);

    // Use the origin position for damage checks
    StartCoroutine(HandleMeleeCone(cone));
}


    private IEnumerator HandleMeleeCone(GameObject cone)
    {
        int damage = CalculateDamage();
        float range = currentWeapon.meleeRange;
        float halfArc = currentWeapon.meleeSwingArc * 0.5f;
        Vector2 forward = new Vector2(lastDirection, 0f);
        Vector3 conePos = cone.transform.position;
        
        // Check for enemies in cone area
        Collider2D[] hits = Physics2D.OverlapCircleAll(conePos, range, enemyLayers);
        
        foreach (Collider2D hit in hits)
        {
            Vector2 toTarget = ((Vector2)hit.transform.position - (Vector2)conePos).normalized;
            
            // Check if enemy is in front of player
            if (Vector2.Dot(forward, toTarget) > 0f)
            {
                // Check if enemy is within the swing arc
                float angle = Vector2.SignedAngle(forward, toTarget);
                if (Mathf.Abs(angle) <= halfArc)
                {
                    ApplyDamage(hit.gameObject, damage);
                }
            }
        }
        
        // Clean up after a short delay (longer if debug is enabled)
        float cleanupDelay = showMeleeCone ? 1f : 0.1f;
        yield return new WaitForSeconds(cleanupDelay);
        if (cone != null)
            Destroy(cone);
    }

    private IEnumerator AnimateWeaponSwing()
    {
        if (weaponVisual == null) yield break;

        isSwinging = true;
        
        float swingDuration = 0.3f;
        float elapsed = 0f;
        
        // Store initial position and rotation
        Vector2 offset = currentWeapon != null ? currentWeapon.weaponOffset : new Vector2(0.5f, 0f);
        Vector3 initialPos = new Vector3(offset.x * lastDirection, offset.y, 0f);
        
        // Start from behind (opposite side) and swing forward
        // When facing right (lastDirection = 1): start at -45, swing to 0
        // When facing left (lastDirection = -1): start at +45, swing to 0
        float startRotation = -45f * lastDirection; // Start from behind
        float endRotation = 0f; // End at forward
        
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;
            
            // Ease out curve for smooth swing
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            
            // Rotate weapon from behind to forward
            float currentRotation = Mathf.Lerp(startRotation, endRotation, easedT);
            weaponVisual.transform.localEulerAngles = new Vector3(0f, 0f, currentRotation);
            
            // Slight forward movement during swing
            float forwardPush = Mathf.Sin(easedT * Mathf.PI) * 0.2f; // Push forward then back
            Vector3 swingPos = new Vector3(
                (offset.x + forwardPush) * lastDirection,
                offset.y,
                0f
            );
            weaponVisual.transform.localPosition = swingPos;
            
            yield return null;
        }
        
        // Reset to initial state
        weaponVisual.transform.localEulerAngles = Vector3.zero;
        weaponVisual.transform.localPosition = initialPos;
        
        isSwinging = false;
    }

    private void PerformDefaultMelee()
    {
        float baseCooldown = 1f / (playerStats != null ? playerStats.AttackSpeed : 1f);
        nextAttackTime = Time.time + baseCooldown;

        // Animate weapon swing
        StartCoroutine(AnimateWeaponSwing());

        // Spawn default melee cone (reuse same logic with default values)
        SpawnDefaultMeleeCone();
    }

    private void SpawnDefaultMeleeCone()
    {
        // Calculate spawn position (same as weapon melee)
        Vector3 spawnPos;
        if (weaponVisual != null && weaponSpriteRenderer != null && weaponSpriteRenderer.sprite != null)
        {
            float weaponWidth = weaponSpriteRenderer.bounds.size.x;
            spawnPos = weaponVisual.transform.position + Vector3.right * (weaponWidth * 0.5f * lastDirection);
        }
        else
        {
            spawnPos = attackPoint != null ? attackPoint.position : transform.position;
        }
        
        GameObject cone = new GameObject("DefaultMeleeCone");
        // Position at weapon (cone will extend from here)
        cone.transform.position = spawnPos;
        
        // Create visual debug representation (always created, visibility controlled by flag)
        CreateConeVisual(cone, 0.5f, 90f); // Default range and arc
        
        StartCoroutine(HandleDefaultMeleeCone(cone));
    }

    private IEnumerator HandleDefaultMeleeCone(GameObject cone)
    {
        int damage = playerStats != null ? playerStats.AttackDamage : 20;
        float range = 0.5f;
        float halfArc = 45f; // 90 degree arc
        Vector2 forward = new Vector2(lastDirection, 0f);
        Vector3 conePos = cone.transform.position;
        
        // Check for enemies in cone area
        Collider2D[] hits = Physics2D.OverlapCircleAll(conePos, range, enemyLayers);
        
        foreach (Collider2D hit in hits)
        {
            Vector2 toTarget = ((Vector2)hit.transform.position - (Vector2)conePos).normalized;
            
            // Check if enemy is in front and within arc
            if (Vector2.Dot(forward, toTarget) > 0f)
            {
                float angle = Vector2.SignedAngle(forward, toTarget);
                if (Mathf.Abs(angle) <= halfArc)
                {
                    ApplyDamage(hit.gameObject, damage);
                }
            }
        }
        
        // Clean up after a short delay (longer if debug is enabled)
        float cleanupDelay = showMeleeCone ? 1f : 0.1f;
        yield return new WaitForSeconds(cleanupDelay);
        if (cone != null)
            Destroy(cone);
    }

    private int CalculateDamage()
    {
        int playerDmg = playerStats != null ? playerStats.AttackDamage : 20;
        
        if (currentWeapon == null)
        {
            return playerDmg; // Default damage
        }
        
        int baseDmg = currentWeapon.baseDamage;
        
        if (currentWeapon.weaponType == WeaponType.Melee)
        {
            // Melee: (base + player) * multiplier
            return Mathf.RoundToInt((baseDmg + playerDmg) * currentWeapon.meleeDamageMultiplier);
        }
        else // Gun
        {
            // Gun: base + player (additive, can add multiplier later if needed)
            return baseDmg + playerDmg;
        }
    }

    private void ApplyDamage(GameObject target, int damage)
    {
        Enemy_Health health = target.GetComponent<Enemy_Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            return;
        }

        MinibossHealth bossHealth = target.GetComponent<MinibossHealth>();
        if (bossHealth != null)
        {
            bossHealth.TakeDamage(damage);
            return;
        }

        BossHeart heartHealth = target.GetComponent<BossHeart>();
        if (heartHealth != null)
        {
            heartHealth.TakeDamage(damage);
        }
    }

    private void CreateConeVisual(GameObject cone, float range, float arc)
    {
        // Create a sprite renderer to visualize the cone
        SpriteRenderer sr = cone.AddComponent<SpriteRenderer>();
        
        // Create a cone-shaped texture (wider at far end, narrow at start)
        int width = 128;
        int height = (int)(range * 64f); // Height based on range
        if (height < 32) height = 32; // Minimum height
        
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        
        float halfArc = arc * 0.5f;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float normalizedY = (float)y / height; // 0 at start (weapon), 1 at end (range)
                float normalizedX = (float)x / width; // 0 to 1 across width
                
                // Calculate cone width at this distance
                // Cone starts narrow (at weapon) and gets wider as it extends
                float coneWidthAtDistance = normalizedY; // 0 at start, 1 at end
                float halfWidth = coneWidthAtDistance * 0.5f;
                
                // Distance from center line
                float distFromCenter = Mathf.Abs(normalizedX - 0.5f);
                
                // Check if within cone bounds
                float alpha = 0f;
                if (distFromCenter <= halfWidth)
                {
                    // Inside cone - calculate alpha based on distance from edges
                    float edgeDist = halfWidth - distFromCenter;
                    alpha = Mathf.Clamp01(edgeDist / (halfWidth * 0.3f)) * 0.4f; // Semi-transparent
                }
                
                pixels[y * width + x] = new Color(1f, 0f, 0f, alpha);
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Create sprite from texture
        // Pivot at bottom center (where weapon is) - (0.5, 0) means bottom center
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0f), // Pivot at bottom center (weapon position)
            64f // Pixels per unit
        );
        
        sr.sprite = sprite;
        sr.color = Color.red;
        sr.sortingOrder = 10;
        
        // Position cone at weapon location (pivot is at bottom, so it extends forward)
        // Scale height to match actual range
        float scaleY = range / (height / 64f);
        float scaleX = (arc / 90f) * 2f; // Scale width based on arc
        cone.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        
        // Rotate to face the correct direction
        // Rotate so the cone extends along +X / -X from the pivot
    float rotationAngle = lastDirection > 0 ? -90f : 90f;
    cone.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);

        // Make visible/invisible based on debug flag
        sr.enabled = showMeleeCone;
    }

    private void OnDrawGizmosSelected()
    {
        if (showMeleeCone && currentWeapon != null && currentWeapon.weaponType == WeaponType.Melee)
        {
            Gizmos.color = Color.red;
            Vector3 pos = attackPoint != null ? attackPoint.position : transform.position;
            
            // Draw cone shape
            float range = currentWeapon.meleeRange;
            float halfArc = currentWeapon.meleeSwingArc * 0.5f;
            Vector2 forward = new Vector2(lastDirection, 0f);
            
            // Draw arc lines
            int segments = 20;
            Vector3 lastPoint = pos;
            for (int i = 0; i <= segments; i++)
            {
                float angle = -halfArc + (halfArc * 2f * i / segments);
                float angleRad = angle * Mathf.Deg2Rad;
                Vector2 direction = new Vector2(
                    Mathf.Cos(angleRad) * lastDirection,
                    Mathf.Sin(angleRad)
                );
                Vector3 point = (Vector2)pos + direction * range;
                
                if (i > 0)
                {
                    Gizmos.DrawLine(lastPoint, point);
                }
                lastPoint = point;
            }
            
            // Draw lines from center to arc ends
            Vector2 leftDir = new Vector2(
                Mathf.Cos(-halfArc * Mathf.Deg2Rad) * lastDirection,
                Mathf.Sin(-halfArc * Mathf.Deg2Rad)
            );
            Vector2 rightDir = new Vector2(
                Mathf.Cos(halfArc * Mathf.Deg2Rad) * lastDirection,
                Mathf.Sin(halfArc * Mathf.Deg2Rad)
            );
            Gizmos.DrawLine(pos, (Vector2)pos + leftDir * range);
            Gizmos.DrawLine(pos, (Vector2)pos + rightDir * range);
        }
    }
}

