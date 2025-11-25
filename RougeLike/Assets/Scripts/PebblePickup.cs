using UnityEngine;

public class PebblePickup : MonoBehaviour
{
    public int amount = 1;
    public float detectRadius = 2f;
    public float suckSpeed = 8f;

    public LogicScript logic;

    Transform player;
    bool flyingToPlayer = false;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;

        GameObject logicObject = GameObject.FindGameObjectWithTag("Logic");
        if (logicObject != null)
            logic = logicObject.GetComponent<LogicScript>();
        
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider == null) return;

        // Ignore collision with the player
        if (p != null)
        {
            Collider2D playerCollider = p.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, playerCollider);
            }
        }

        // Ignore collision with all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, enemyCollider, true); // Ignore collision
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        if (!flyingToPlayer)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= detectRadius)
            {
                flyingToPlayer = true;
            }
        }

        if (flyingToPlayer)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                suckSpeed * Time.deltaTime
            );

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist < 0.75f)
            {
                if (PebbleManager.Instance != null)
                    PebbleManager.Instance.AddPebbles(amount);

                Destroy(gameObject);
                logic.UpdatePebbleCounter();
            }
        }
    }
}
