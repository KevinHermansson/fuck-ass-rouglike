using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public float attackCooldown;
    public float startAttackCooldown;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;
    public GameObject projectile;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;
    private float lastDirection = 1f;
    private Vector3 attackPointOffset; // Store the original offset from player
    private Player_Stats playerStats;
    private float nextAttackTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the initial offset (should be positive X for right side)
        if (attackPoint != null)
        {
            attackPointOffset = attackPoint.localPosition;
        }
        playerStats = GetComponent<Player_Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        // Track direction based on input
        if (Input.GetKey(KeyCode.D))
            lastDirection = 1f;
        else if (Input.GetKey(KeyCode.A))
            lastDirection = -1f;

        // Flip attack point based on direction
        if (attackPoint != null)
        {
            // If moving left (lastDirection = -1), flip the X position
            attackPoint.localPosition = new Vector3(
                Mathf.Abs(attackPointOffset.x) * lastDirection,
                attackPointOffset.y,
                attackPointOffset.z
            );
        }

        // Use attack speed to determine cooldown
        float currentAttackSpeed = (playerStats != null) ? playerStats.AttackSpeed : 1f;
        float cooldown = 1f / currentAttackSpeed; // Higher attack speed = lower cooldown

        if (Input.GetKeyDown(KeyCode.K) && Time.time >= nextAttackTime)
        {
            meleeAttack();
            nextAttackTime = Time.time + cooldown;
        }
        if (Input.GetKeyDown(KeyCode.J) && Time.time >= nextAttackTime)
        {
            rangedAttack();
            nextAttackTime = Time.time + cooldown;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void meleeAttack()
    {
        int currentDamage = (playerStats != null) ? playerStats.AttackDamage : attackDamage;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        for (int i = 0; i < hitEnemies.Length; i++)
        {
            Enemy_Health health = hitEnemies[i].GetComponent<Enemy_Health>();
            if (health != null)
            {
                health.TakeDamage(currentDamage);
            }
        }
        Debug.Log($"Player Attacked with {currentDamage} damage!");
    }

    public void rangedAttack()
    {
        if (projectile != null)
        {
            GameObject proj = Instantiate(projectile, attackPoint.position, Quaternion.identity);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(lastDirection * projectileSpeed, 0f);
            }
            StartCoroutine(CheckProjectileCollision(proj));
            Destroy(proj, projectileLifetime);
            Debug.Log("Ranged Attack!");
        }
    }

    private IEnumerator CheckProjectileCollision(GameObject proj)
    {
        float elapsed = 0f;
        float checkRadius = 0.2f; // adjust to match projectile size
        while (proj != null && elapsed < projectileLifetime)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(proj.transform.position, checkRadius, enemyLayers);
            if (hits.Length > 0)
            {
                int currentDamage = (playerStats != null) ? playerStats.AttackDamage : attackDamage;
                for (int i = 0; i < hits.Length; i++)
                {
                    Enemy_Health h = hits[i].GetComponent<Enemy_Health>();
                    if (h != null)
                        h.TakeDamage(currentDamage);
                }
                Destroy(proj);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}



