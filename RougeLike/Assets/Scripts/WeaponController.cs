using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private RegularInventoryHolder inventoryHolder;
    
    [Header("Settings")]
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0; // Left mouse button
    [SerializeField] private bool allowHoldToShoot = true;
    
    private WeaponItem currentWeapon;
    private int currentAmmo;
    private float nextFireTime = 0f;
    private float lastDirection = 1f;
    private Player_Stats playerStats;
    private MovementScript movementScript;
    
    // Events for HUD updates
    public System.Action<WeaponItem, int> OnWeaponChanged;
    public System.Action<int, int> OnAmmoChanged; // current, max
    
    private void Start()
    {
        playerStats = GetComponent<Player_Stats>();
        movementScript = GetComponent<MovementScript>();
        
        if (inventoryHolder == null)
            inventoryHolder = GetComponent<RegularInventoryHolder>();
        
        if (firePoint == null)
        {
            // Try to find or create fire point
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = new Vector3(0.5f, 0f, 0f);
            firePoint = firePointObj.transform;
        }
        
        // Subscribe to inventory changes to update weapon when selection changes
        if (inventoryHolder != null && inventoryHolder.Inventory != null)
        {
            inventoryHolder.Inventory.OnChanged += OnInventoryChanged;
            OnInventoryChanged();
        }
    }
    
    private void OnDestroy()
    {
        if (inventoryHolder != null && inventoryHolder.Inventory != null)
        {
            inventoryHolder.Inventory.OnChanged -= OnInventoryChanged;
        }
    }
    
    private void Update()
    {
        // Track direction based on input
        if (Input.GetKey(KeyCode.D))
            lastDirection = 1f;
        else if (Input.GetKey(KeyCode.A))
            lastDirection = -1f;
        
        // Update fire point position based on direction
        if (firePoint != null)
        {
            firePoint.localPosition = new Vector3(
                Mathf.Abs(firePoint.localPosition.x) * lastDirection,
                firePoint.localPosition.y,
                firePoint.localPosition.z
            );
        }
        
        // Handle shooting
        if (currentWeapon != null)
        {
            bool canShoot = Time.time >= nextFireTime;
            bool wantsToShoot = allowHoldToShoot ? Input.GetKey(shootKey) : Input.GetKeyDown(shootKey);
            
            if (wantsToShoot && canShoot)
            {
                TryShoot();
            }
        }
    }
    
    private void OnInventoryChanged()
    {
        if (inventoryHolder == null || inventoryHolder.Inventory == null) return;
        
        int selectedIndex = inventoryHolder.Inventory.SelectedIndex;
        var selectedItem = inventoryHolder.Inventory.GetAt(selectedIndex);
        
        if (selectedItem is WeaponItem weapon)
        {
            EquipWeapon(weapon);
        }
        else
        {
            UnequipWeapon();
        }
    }
    
    public void EquipWeapon(WeaponItem weapon)
    {
        if (weapon == null) return;
        
        currentWeapon = weapon;
        currentAmmo = weapon.startingAmmo;
        nextFireTime = 0f;
        
        OnWeaponChanged?.Invoke(weapon, currentAmmo);
        OnAmmoChanged?.Invoke(currentAmmo, weapon.maxAmmo);
        
        Debug.Log($"Equipped weapon: {weapon.DisplayName} ({currentAmmo}/{weapon.maxAmmo})");
    }
    
    public void UnequipWeapon()
    {
        currentWeapon = null;
        currentAmmo = 0;
        OnWeaponChanged?.Invoke(null, 0);
        OnAmmoChanged?.Invoke(0, 0);
    }
    
    private void TryShoot()
    {
        if (currentWeapon == null) return;
        
        // Check ammo
        if (currentWeapon.usesAmmo && !currentWeapon.infiniteAmmo)
        {
            if (currentAmmo <= 0)
            {
                // Out of ammo - could play a click sound here
                return;
            }
            currentAmmo--;
        }
        
        // Calculate fire rate cooldown
        float fireCooldown = 1f / currentWeapon.fireRate;
        nextFireTime = Time.time + fireCooldown;
        
        // Shoot
        Shoot();
        
        // Update HUD
        OnAmmoChanged?.Invoke(currentAmmo, currentWeapon.maxAmmo);
    }
    
    private void Shoot()
    {
        if (currentWeapon == null || firePoint == null) return;
        
        if (currentWeapon.projectilePrefab != null)
        {
            // Spawn projectile
            GameObject projectile = Instantiate(
                currentWeapon.projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );
            
            // Set projectile velocity
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(lastDirection * currentWeapon.projectileSpeed, 0f);
            }
            
            // Set projectile damage
            int damage = currentWeapon.damage;
            if (playerStats != null)
            {
                damage = playerStats.AttackDamage + currentWeapon.damage;
            }
            
            // Add projectile component if it doesn't exist
            Projectile projComponent = projectile.GetComponent<Projectile>();
            if (projComponent == null)
            {
                projComponent = projectile.AddComponent<Projectile>();
            }
            projComponent.Initialize(damage, enemyLayers, currentWeapon.projectileLifetime);
            
            // Destroy projectile after lifetime
            Destroy(projectile, currentWeapon.projectileLifetime);
        }
        else
        {
            // Raycast-based shooting (if no projectile prefab)
            RaycastHit2D hit = Physics2D.Raycast(
                firePoint.position,
                new Vector2(lastDirection, 0f),
                currentWeapon.range,
                enemyLayers
            );
            
            if (hit.collider != null)
            {
                int damage = currentWeapon.damage;
                if (playerStats != null)
                {
                    damage = playerStats.AttackDamage + currentWeapon.damage;
                }
                
                Enemy_Health enemyHealth = hit.collider.GetComponent<Enemy_Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
                
                MinibossHealth bossHealth = hit.collider.GetComponent<MinibossHealth>();
                if (bossHealth != null)
                {
                    bossHealth.TakeDamage(damage);
                }
            }
        }
    }
    
    public void AddAmmo(int amount)
    {
        if (currentWeapon == null) return;
        
        currentAmmo = Mathf.Min(currentAmmo + amount, currentWeapon.maxAmmo);
        OnAmmoChanged?.Invoke(currentAmmo, currentWeapon.maxAmmo);
    }
    
    public WeaponItem GetCurrentWeapon()
    {
        return currentWeapon;
    }
    
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
}

