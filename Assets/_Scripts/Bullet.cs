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
        // Şimdilik sadece duvarlara çarpınca yok olmasını sağlayalım.
        // İleride buraya "Enemy" tag'i de ekleyeceğiz.
        if(hitInfo.CompareTag("Environment")) // (Haritanın sınırlarına Environment tag'i verebilirsin)
        {
            CancelInvoke("Deactivate"); // Çarptığı için süreyi beklemesine gerek kalmadı
            Deactivate();
        }
    }
}