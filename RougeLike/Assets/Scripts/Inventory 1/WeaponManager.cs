using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    
    private Player_Stats playerStats;
    private float lastDirection = 1f;
    private float nextAttackTime = 0f;
    private WeaponData currentWeapon;
    private int currentWeaponSlot = -1;
    private SpriteRenderer weaponSpriteRenderer;
    private GameObject weaponVisual;

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

        if (currentWeapon != null && currentWeapon.weaponSprite != null)
        {
            weaponVisual.SetActive(true);
            weaponSpriteRenderer.sprite = currentWeapon.weaponSprite;
            
            // Position weapon in front of player
            Vector2 offset = currentWeapon.weaponOffset;
            offset.x *= lastDirection; // Flip offset based on direction
            weaponVisual.transform.localPosition = new Vector3(offset.x, offset.y, 0f);
            
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

        Vector3 attackPos = attackPoint != null ? attackPoint.position : transform.position;
        float range = currentWeapon.meleeRange;
        int damage = CalculateDamage();

        // Spawn slash effect
        if (currentWeapon.slashEffectPrefab != null)
        {
            GameObject slash = Instantiate(currentWeapon.slashEffectPrefab, attackPos, Quaternion.identity);
            
            // Flip slash based on direction
            if (lastDirection < 0)
            {
                Vector3 scale = slash.transform.localScale;
                scale.x *= -1;
                slash.transform.localScale = scale;
            }
            
            // Destroy slash effect after animation
            Destroy(slash, 0.5f);
        }

        // Check for enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, range, enemyLayers);
        
        for (int i = 0; i < hitEnemies.Length; i++)
        {
            ApplyDamage(hitEnemies[i].gameObject, damage);
        }
    }

    private void PerformDefaultMelee()
    {
        float baseCooldown = 1f / (playerStats != null ? playerStats.AttackSpeed : 1f);
        nextAttackTime = Time.time + baseCooldown;

        Vector3 attackPos = attackPoint != null ? attackPoint.position : transform.position;
        int damage = playerStats != null ? playerStats.AttackDamage : 20;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, 0.5f, enemyLayers);
        
        for (int i = 0; i < hitEnemies.Length; i++)
        {
            ApplyDamage(hitEnemies[i].gameObject, damage);
        }
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

    private void OnDrawGizmosSelected()
    {
        if (currentWeapon != null && currentWeapon.weaponType == WeaponType.Melee)
        {
            Gizmos.color = Color.red;
            Vector3 pos = attackPoint != null ? attackPoint.position : transform.position;
            Gizmos.DrawWireSphere(pos, currentWeapon.meleeRange);
        }
    }
}

