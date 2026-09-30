using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input Sistemi eklendi

public class PlayerShooting : MonoBehaviour
{
    [Header("Silah Ayarları")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.15f;
    private float nextFireTime;

    [Header("Havuz (Pool) Ayarları")]
    public int poolSize = 50;
    private List<GameObject> bulletPool;

    private PlayerController playerController;
    private bool isShooting;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        InitializePool();
    }

    // OYUN BAŞLADIĞINDA MERMİLERİ ÜRETİP SAKLIYORUZ
    void InitializePool()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false); // Başlangıçta görünmez yap
            bulletPool.Add(obj);
        }
    }

    // =================================================================
    // YENİ INPUT SİSTEMİ: Sol Tık (Fare) veya Right Bumper [RB] (Gamepad)
    // =================================================================
    void OnShoot(InputValue value)
    {
        // isPressed, tuşa basılı tutulduğu sürece true, bırakıldığında false olur
        isShooting = value.isPressed;
    }

    void Update()
    {
        // Taramalı mantığı: Basılı tutuyorsa, bekleme süresi dolduysa VE DASH ATMIYORSA ateş et
        if (isShooting && Time.time >= nextFireTime && !playerController.isDashing)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // Havuzdan (Pool) uyuyan bir mermi bul
        GameObject bullet = GetPooledBullet();

        if (bullet != null)
        {
            // Mermiyi namlunun ucuna (FirePoint) taşı
            bullet.transform.position = firePoint.position;

            // ARTIK FARE HESABI YOK!
            // Karakter (PlayerController) zaten fareye veya Gamepad'e doğru döndüğü için,
            // sadece karakterin baktığı yönü (transform.up) almamız yeterli.
            Vector2 fireDirection = transform.up;

            // Mermiyi uyandır ve Bullet.cs içindeki Fire fonksiyonunu tetikle
            bullet.SetActive(true);
            bullet.GetComponent<Bullet>().Fire(fireDirection);
        }
    }

    // HAVUZDA BOŞTA OLAN İLK MERMİYİ BULAN FONKSİYON
    GameObject GetPooledBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                return bulletPool[i];
            }
        }
        return null; // Eğer havuzda mermi kalmadıysa ateş etmez
    }
}