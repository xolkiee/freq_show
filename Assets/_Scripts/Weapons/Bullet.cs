using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f; 
    
    private Rigidbody2D rb;
    
    private int currentPierceCount; 
    private int currentRicochetCount; // YENİ: Sekme hakkı hafızası

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // YENİ: Fire fonksiyonu artık ricochetCount'u da alıyor
    public void Fire(Vector2 direction, int pierceCount, int ricochetCount)
    {
        currentPierceCount = pierceCount; 
        currentRicochetCount = ricochetCount; 
        
        rb.velocity = direction * speed;
        
        // Merminin rotasyonunu da gidiş yönüne çevirelim ki sektiğinde ucu da dönsün
        transform.up = direction; 
        
        Invoke("Deactivate", lifeTime); 
    }

    void Deactivate()
    {
        gameObject.SetActive(false); 
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Enemy"))
        {
            EnemyHealth enemy = hitInfo.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); 
            }
            
            // 1. Önce Delme kontrolü (Piercing)
            if (currentPierceCount > 0)
            {
                currentPierceCount--; 
            }
            // 2. Delme bittiyse veya yoksa, Sekme kontrolü (Ricochet)
            else if (currentRicochetCount > 0)
            {
                BounceToNearestEnemy(hitInfo.transform.position);
            }
            // 3. Hiçbiri yoksa Klasik mermi (Yok ol)
            else
            {
                Deactivate(); 
            }
        }
        else if (hitInfo.CompareTag("Environment"))
        {
            // Duvara çarptıysa sadece Sekme (Ricochet) çalışır, duvar delinemez
            if (currentRicochetCount > 0)
            {
                BounceToNearestEnemy(transform.position);
            }
            else
            {
                Deactivate();
            }
        }
    }

    // YENİ: En yakındaki düşmanı bulup mermiyi oraya çeviren fizik motoru
    void BounceToNearestEnemy(Vector3 currentPos)
    {
        currentRicochetCount--; // Sekme hakkını 1 azalt

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(currentPos, enemy.transform.position);
            
            // 0.1f'den büyük olma şartı: Çarptığımız düşmanın kendi içinde sekmeyi engeller
            if (distance < minDistance && distance > 0.1f) 
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            // Yeni yönü hesapla ve mermiyi o yöne fırlat
            Vector2 newDirection = (closestEnemy.transform.position - currentPos).normalized;
            rb.velocity = newDirection * speed;
            transform.up = newDirection; // Merminin burnunu yeni hedefe çevir
            
            // Havuz süresini yenile ki seken mermi hemen havada kaybolmasın
            CancelInvoke("Deactivate");
            Invoke("Deactivate", lifeTime);
        }
        else
        {
            // Etrafta sekecek başka düşman yoksa yok ol
            Deactivate();
        }
    }

    void OnDisable()
    {
        CancelInvoke("Deactivate");
    }
}