using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stoppingDistance = 10f; // YENİ: Düşmanın duracağı mesafe
    
    private Transform playerTarget;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Karakterini bul (Senin güncellediğin Tag olan "Player"ı yazdım)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (playerTarget == null) return;

        // Oyuncuyla düşman arasındaki tam mesafeyi ölç
        float distance = Vector2.Distance(transform.position, playerTarget.position);

        // Eğer aradaki mesafe belirlediğimiz sınırdan BÜYÜKSE kovalamaya devam et
        if (distance > stoppingDistance)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            // Düşmanın yüzünü döndürme
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
        else 
        {
            // Yeterince yakınsa dur! (Freni çek)
            rb.velocity = Vector2.zero;
        }
    }
}