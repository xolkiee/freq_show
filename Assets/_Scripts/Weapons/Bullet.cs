using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f; // 2 saniye hiçbir şeye çarpmazsa silinsin (havuza dönsün)
    
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Bu fonksiyonu Player ateş ettiğinde çağıracağız
    public void Fire(Vector2 direction)
    {
        rb.velocity = direction * speed;
        Invoke("Deactivate", lifeTime); // Havuza geri dönme zamanlayıcısını başlat
    }

    void Deactivate()
    {
        gameObject.SetActive(false); // Destroy YERİNE SetActive(false) yapıyoruz! Sırrımız bu.
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. Eğer çarptığımız şeyin etiketi "Enemy" ise
        if (hitInfo.CompareTag("Enemy"))
        {
            // Düşmanın can kodunu bul
            EnemyHealth enemy = hitInfo.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // 1 Hasar ver
            }
            
            CancelInvoke("Deactivate");
            Deactivate(); // Mermiyi yok et (Havuza geri gönder)
        }
        // 2. Eğer duvara (Environment) çarptıysa sadece mermiyi yok et
        else if (hitInfo.CompareTag("Environment"))
        {
            CancelInvoke("Deactivate");
            Deactivate();
        }
    }
}