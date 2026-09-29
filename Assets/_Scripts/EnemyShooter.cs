using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public float fireRate = 2f; // 2 saniyede bir mermi atsın
    private float nextFireTime;
    private Transform playerTarget;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
        
        // Hepsi aynı anda ateş etmesin diye ufak bir rastgelelik ekliyoruz
        nextFireTime = Time.time + Random.Range(0.5f, 2f); 
    }

    void Update()
    {
        if (playerTarget == null) return;

        // Düşman oyuncuya belirli bir mesafeden (örneğin 10 birim) yakınsa ateş etsin
        if (Vector2.Distance(transform.position, playerTarget.position) < 10f)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        // Singleton sayesinde Merkez Bankasından direkt mermi çekiyoruz!
        GameObject bullet = EnemyBulletPool.Instance.GetBullet();
        
        if (bullet != null)
        {
            bullet.transform.position = transform.position; // Mermi düşmanın içinden çıksın
            
            // Oyuncuya doğru yön hesapla
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            
            bullet.SetActive(true);
            bullet.GetComponent<EnemyBullet>().Fire(direction);
        }
    }
}