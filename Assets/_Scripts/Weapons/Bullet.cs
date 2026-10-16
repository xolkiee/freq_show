using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float baseSpeed = 20f; 
    public float lifeTime = 2f; 
    
    private Rigidbody2D rb;
    
    private int currentPierceCount; 
    private int currentRicochetCount;
    private int currentCannonballLevel;
    private int currentShrapnelLevel;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Fire(Vector2 direction, int pierceCount, int ricochetCount, int cannonLevel, int shrapnelLevel, bool isShrapnel = false)
    {
        currentPierceCount = pierceCount; 
        currentRicochetCount = ricochetCount; 
        currentCannonballLevel = cannonLevel;
        currentShrapnelLevel = shrapnelLevel;
        
        float actualSpeed = currentCannonballLevel > 0 ? baseSpeed * 0.5f : baseSpeed;
        
        // ÇÖZÜM: Eğer bu bir şarapnelse boyutunu ufak tut, değilse gülle/normal mermi kuralını uygula
        if (isShrapnel)
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        }
        else
        {
            transform.localScale = currentCannonballLevel > 0 ? new Vector3(1.5f, 1.5f, 1f) : new Vector3(1f, 1f, 1f);
        }

        rb.velocity = direction * actualSpeed;
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
                // Gülle ekstra hasar verebilir (örneğin seviye başına +1 hasar)
                int damage = currentCannonballLevel > 0 ? 1 + currentCannonballLevel : 1;
                enemy.TakeDamage(damage); 
            }

           // GÜLLE İTME (KNOCKBACK) MANTIĞI
            if (currentCannonballLevel > 0)
            {
                EnemyAI enemyAI = hitInfo.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    // İtme yönü ve gücünü ayarla (20f genelde iyi bir değerdir, test edip değiştirebilirsin)
                    Vector2 knockbackForce = rb.velocity.normalized * (currentCannonballLevel * 20f);
                    
                    // Düşmanı 0.2 saniye boyunca yürümekten alıkoy ve savur
                    enemyAI.ApplyKnockback(knockbackForce, 0.2f); 
                }
            }
            
            ProcessImpact(hitInfo.transform.position);
        }
        else if (hitInfo.CompareTag("Environment"))
        {
            ProcessImpact(transform.position);
        }
    }

    // Merminin çarpma sonrası karar mekanizması
    void ProcessImpact(Vector3 impactPosition)
    {
        if (currentPierceCount > 0)
        {
            currentPierceCount--; 
        }
        else if (currentRicochetCount > 0)
        {
            BounceToNearestEnemy(impactPosition);
        }
        else
        {
            // Ne delme ne sekme kaldıysa yok olmalı. Ama önce Şarapnel kontrolü yapıyoruz.
            if (currentShrapnelLevel > 0)
            {
                ExplodeIntoShrapnel();
            }
            Deactivate(); 
        }
    }

    // ŞARAPNEL MANTIĞI: Etrafa küçük mermiler saç
    void ExplodeIntoShrapnel()
    {
        int shrapnelCount = currentShrapnelLevel * 3; 
        float angleStep = 360f / shrapnelCount;

        for (int i = 0; i < shrapnelCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // ÇÖZÜM: Parçacıkları merkezde (transform.position) değil, dir yönünde 0.5 birim dışarıda başlatıyoruz!
            Vector3 spawnPos = transform.position + (Vector3)(dir * 0.5f);
            
            GameObject shrapnel = Instantiate(gameObject, spawnPos, Quaternion.identity);
            
            shrapnel.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            shrapnel.GetComponent<Bullet>().baseSpeed = baseSpeed * 0.8f; 
            shrapnel.GetComponent<Bullet>().Fire(dir, 0, 0, 0, 0, true); 
        }
    }

    void BounceToNearestEnemy(Vector3 currentPos)
    {
        currentRicochetCount--; 

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(currentPos, enemy.transform.position);
            
            if (distance < minDistance && distance > 0.1f) 
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            Vector2 newDirection = (closestEnemy.transform.position - currentPos).normalized;
            rb.velocity = newDirection * (currentCannonballLevel > 0 ? baseSpeed * 0.5f : baseSpeed);
            transform.up = newDirection; 
            
            CancelInvoke("Deactivate");
            Invoke("Deactivate", lifeTime);
        }
        else
        {
            if (currentShrapnelLevel > 0) ExplodeIntoShrapnel();
            Deactivate();
        }
    }

    void OnDisable()
    {
        CancelInvoke("Deactivate");
    }
}