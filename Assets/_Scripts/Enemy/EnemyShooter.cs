using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public float fireRate = 2f;
    private float nextFireTime;

    // Artık kendi hedefini aramak yerine EnemyAI'ın beynini okuyacak
    private EnemyAI enemyAI;

    void Start()
    {
        // Aynı obje üzerindeki EnemyAI kodunu bul ve bağlan
        enemyAI = GetComponent<EnemyAI>();

        // Hepsi aynı anda ateş etmesin diye ufak bir rastgelelik ekliyoruz
        nextFireTime = Time.time + Random.Range(0.5f, 2f);
    }

    void Update()
    {
        // Eğer beyin (EnemyAI) yoksa veya henüz bir hedef bulamadıysa hiçbir şey yapma
        if (enemyAI == null || enemyAI.playerTarget == null) return;

        // Beynin o an kilitlendiği (en yakındaki) güncel hedefi al
        Transform currentTarget = enemyAI.playerTarget;

        // Düşman oyuncuya belirli bir mesafeden (örneğin 10 birim) yakınsa ateş etsin
        if (Vector2.Distance(transform.position, currentTarget.position) < 10f)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                Shoot(currentTarget);
            }
        }
    }

    // Shoot fonksiyonu artık kime ateş edeceğini dışarıdan (parametre olarak) alıyor
    void Shoot(Transform target)
    {
        // Singleton sayesinde Merkez Bankasından direkt mermi çekiyoruz!
        GameObject bullet = EnemyBulletPool.Instance.GetBullet();

        if (bullet != null)
        {
            bullet.transform.position = transform.position;

            // Güncel hedefe doğru yön hesapla
            Vector2 direction = (target.position - transform.position).normalized;

            bullet.SetActive(true);
            bullet.GetComponent<EnemyBullet>().Fire(direction);
        }
    }
}