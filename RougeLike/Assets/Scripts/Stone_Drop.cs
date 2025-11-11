using UnityEngine;

public class Stone_Drop : MonoBehaviour
{
    public float dropSpeed = 5f; 
    public int damage = 10; 
    

    private Rigidbody2D rb; 
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
        
       
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Säkerställ att Rigidbody2D är korrekt konfigurerad
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Viktigt för snabba objekt!
        rb.linearVelocity = Vector2.down * dropSpeed;

        // Kolla om stenen har en Collider2D
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(0.5f, 0.5f); // Sätt en standardstorlek
        }

    }

    void Update()
    {
        // Förstör stenen om den lever för länge (säkerhetsnät)
       
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        

        // Kolla om stenen träffar marken (flera möjliga taggar)
        string tag = collision.gameObject.tag;
        if (tag == "Ground" || tag == "fancyPlatform" || tag == "Untagged")
        {

            Destroy(gameObject); // Förstör stenen
            return;
        }

        
    }
}
