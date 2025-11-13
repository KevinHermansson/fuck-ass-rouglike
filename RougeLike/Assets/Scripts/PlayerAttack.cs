using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    private float attackCooldown;
    public float startAttackCooldown;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCooldown <= 0)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
                for (int i = 0; i < hitEnemies.Length; i++)
                {
                    EnemyHealth enemyHealth = hitEnemies[i].GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(attackDamage);
                    }
                }
                // Attack code here 
                Debug.Log("Player Attacked!");
            }
            attackCooldown = startAttackCooldown;
        }
        else
        {
            attackCooldown -= Time.deltaTime;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

