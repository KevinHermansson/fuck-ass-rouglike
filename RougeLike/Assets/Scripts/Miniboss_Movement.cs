using UnityEngine;
using System.Collections;

public class Miniboss_Movement : MonoBehaviour
{
    public Animator animator;
    public float teleportInterval = 10f;
    public float teleportAnimationDuration = 1f; // How long the teleport animation takes
    public Vector2 teleportAreaMin = new Vector2(-8f, -4f); // Minimum teleport position
    public Vector2 teleportAreaMax = new Vector2(8f, 4f); // Maximum teleport position
    public float attackInterval = 7f; // How often to attack
    public float attackAnimationDuration = 1f; // How long the attack animation takes
    
    private float teleportTimer = 0f;
    private float attackTimer = 0f;
    private bool isTeleporting = false;
    private bool isAttacking = false;




    void Start()
    {
        animator = GetComponent<Animator>();
        teleportTimer = teleportInterval; // Start timer
        attackTimer = attackInterval; // Start attack timer
    }

    void Update()
    {
        if (isTeleporting || isAttacking) return; // Don't update timers while busy

        // Count down teleport timer
        teleportTimer -= Time.deltaTime;

        // Teleport when timer reaches 0
        if (teleportTimer <= 0f)
        {
            StartCoroutine(TeleportSequence());
            teleportTimer = teleportInterval; // Reset timer
        }

        // Count down attack timer
        attackTimer -= Time.deltaTime;

        // Attack when timer reaches 0
        if (attackTimer <= 0f)
        {
            StartCoroutine(AttackSequence());
            attackTimer = attackInterval; // Reset timer
        }
    }

    IEnumerator TeleportSequence()
    {
        isTeleporting = true;

        // Play teleport animation (disappear)
        if (animator != null)
        {
            animator.SetBool("Teleport", true);
        }

        // Wait for the teleport animation to complete (disappear phase)
        yield return new WaitForSeconds(teleportAnimationDuration * 2f);

        // Set Teleport to false
        if (animator != null)
        {
            animator.SetBool("Teleport", false);
        }

        // Move to random position (only change X, keep Y the same)
        // Calculate camera bounds
        Camera cam = Camera.main;
        float cameraHeight = 2f * cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;
        float minX = cam.transform.position.x - cameraWidth / 2f;
        float maxX = cam.transform.position.x + cameraWidth / 2f;

        // Get random position within camera bounds
        float randomX = Random.Range(minX + 1f, maxX - 1f); // Added 1f margin to keep it away from edges
        transform.position = new Vector3(randomX, transform.position.y, transform.position.z);

        // Play spawn animation (reappear)
        if (animator != null)
        {
            animator.SetBool("Spawn", true);
        }

        // Wait for the spawn animation to complete (reappear phase)
        yield return new WaitForSeconds(teleportAnimationDuration * 1f);

        // Set Spawn to false
        if (animator != null)
        {
            animator.SetBool("Spawn", false);
        }

        isTeleporting = false;
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;

        // Randomly choose between Attack1 and Attack2
        int randomAttack = Random.Range(1, 3); // Returns 1 or 2

        if (randomAttack == 1)
        {
            yield return StartCoroutine(Attack1Sequence());
        }
        else
        {
            yield return StartCoroutine(Attack2Sequence());
        }

        isAttacking = false;
    }

    IEnumerator Attack1Sequence()
    {
        // Play Attack1 animation
        if (animator != null)
        {
            animator.SetBool("Attack1", true);
        }

        // Wait for the attack animation to complete
        yield return new WaitForSeconds(attackAnimationDuration);

        // Set Attack1 to false
        if (animator != null)
        {
            animator.SetBool("Attack1", false);
        }
    }

    IEnumerator Attack2Sequence()
    {
        // Play Attack2 animation
        if (animator != null)
        {
            animator.SetBool("Attack2", true);
        }

        // Wait for the attack animation to complete
        yield return new WaitForSeconds(attackAnimationDuration);

        // Set Attack2 to false
        if (animator != null)
        {
            animator.SetBool("Attack2", false);
        }
    }
}
