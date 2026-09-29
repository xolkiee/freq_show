using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Silah Ayarları")]
    public GameObject bulletPrefab; // Oluşturduğun Prefab'ı buraya sürükleyeceksin
    public Transform firePoint;     // Merminin çıkacağı nokta
    public float fireRate = 0.15f;  // Saniyede kaç mermi? (Düşürdükçe hızlanır)
    private float nextFireTime;

    [Header("Havuz (Pool) Ayarları")]
    public int poolSize = 50; // Aynı anda ekranda olabilecek maksimum mermi
    private List<GameObject> bulletPool;

    private Camera mainCam;
    private PlayerController playerController;
    private bool isShooting;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        mainCam = Camera.main;
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

    void Update()
    {
        // Yeni Input System ile Farenin Sol Tuşunu (veya Gamepad) oku
        if (Mouse.current != null)
        {
            isShooting = Mouse.current.leftButton.isPressed;
        }

        // Taramalı mantığı: Basılı tutuyorsa ve bekleme süresi dolduysa ateş et
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
            
            // Farenin olduğu yere doğru yön vektörü hesapla
            Vector2 mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 fireDirection = (mousePos - (Vector2)firePoint.position).normalized;

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
        return null; // Eğer havuzda mermi kalmadıysa ateş etmez (Mermi cehennemini korur)
    }
}