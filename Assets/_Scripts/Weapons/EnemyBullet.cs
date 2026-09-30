using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 7f;
    public float lifeTime = 3f;
    public int damageAmount = 10;

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
        // 1. Önce kime çarptığına bak. Eğer Oyuncuysa DOĞRUDAN ONA HASAR VER.
        if (hitInfo.CompareTag("Player"))
        {
            PlayerHealth hitPlayer = hitInfo.GetComponent<PlayerHealth>();

            // Çarptığımız objede PlayerHealth kodu varsa TakeDamage fonksiyonunu tetikle
            if (hitPlayer != null)
            {
                hitPlayer.TakeDamage(damageAmount);
            }
        }

        // 2. Çarptığı şey ister Oyuncu ister Duvar olsun, mermiyi kapat.
        if (hitInfo.CompareTag("Player") || hitInfo.CompareTag("Environment"))
        {
            CancelInvoke("Deactivate");
            Deactivate();
        }
    }
}