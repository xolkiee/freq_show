using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 7f;
    public float lifeTime = 3f;
    public int damageAmount = 10; // Merminin vuracağı hasar miktarını buradan ayarlayabilirsin

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
        // 1. Önce kime çarptığına bak. Eğer Oyuncuysa hasar sinyali yolla.
        // (Eğer oyuncu o an Dodge atıyorsa, PlayerHealth kodu bu hasarı zaten iptal edecek)
        if (hitInfo.CompareTag("Player"))
        {
            GameEvents.TriggerDamageAttempt(damageAmount);
        }

        // 2. Çarptığı şey ister Oyuncu ister Duvar (Environment) olsun, mermiyi kapat.
        if (hitInfo.CompareTag("Player") || hitInfo.CompareTag("Environment"))
        {
            CancelInvoke("Deactivate");
            Deactivate();
        }
    }
}