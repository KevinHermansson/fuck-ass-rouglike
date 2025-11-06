using UnityEngine;

public class PebblePickup : MonoBehaviour
{
    public int amount = 1;
    public float detectRadius = 2f;
    public float suckSpeed = 8f;

    Transform player;
    bool flyingToPlayer = false;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;
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
            if (dist < 0.25f)
            {
                if (PebbleManager.Instance != null)
                    PebbleManager.Instance.AddPebbles(amount);

                Destroy(gameObject);
            }
        }
    }
}
