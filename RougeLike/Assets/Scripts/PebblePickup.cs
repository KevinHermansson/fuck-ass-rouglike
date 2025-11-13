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
