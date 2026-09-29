using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 7f; // Bizim mermimizden biraz daha yavaş olsun ki kaçabilelim
    public float lifeTime = 3f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Fire(Vector2 direction)
    {
        rb.velocity = direction * speed;
        Invoke("Deactivate", lifeTime);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

   void OnTriggerEnter2D(Collider2D hitInfo)
    {
    
        if (hitInfo.CompareTag("Player") || hitInfo.CompareTag("Environment"))
        {
            CancelInvoke("Deactivate");
            Deactivate();
        }
    }
}