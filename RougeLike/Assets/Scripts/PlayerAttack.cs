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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the initial offset (should be positive X for right side)
        if (attackPoint != null)
        {
            attackPointOffset = attackPoint.localPosition;
        }
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

        if (Input.GetKeyDown(KeyCode.K))
            meleeAttack();
        if (Input.GetKeyDown(KeyCode.J))
            rangedAttack();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void meleeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        for (int i = 0; i < hitEnemies.Length; i++)
        {
            Enemy_Health health = hitEnemies[i].GetComponent<Enemy_Health>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }
        Debug.Log("Player Attacked!");
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
                for (int i = 0; i < hits.Length; i++)
                {
                    Enemy_Health h = hits[i].GetComponent<Enemy_Health>();
                    if (h != null)
                        h.TakeDamage(attackDamage);
                }
                Destroy(proj);
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}



