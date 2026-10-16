using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

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
    private PlayerUpgradeManager upgradeManager; // YENİ EKLENDİ
    private bool isShooting;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        upgradeManager = GetComponent<PlayerUpgradeManager>(); // YENİ EKLENDİ
        InitializePool();
    }

    void InitializePool()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false); 
            bulletPool.Add(obj);
        }
    }

    void OnShoot(InputValue value)
    {
        isShooting = value.isPressed;
    }

    void Update()
    {
        if (isShooting && Time.time >= nextFireTime && !playerController.isDashing)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = GetPooledBullet();

        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            Vector2 fireDirection = transform.up;

            // YENİ: İki yeteneğin de o anki seviyesini (değerini) hafızadan çekiyoruz
            int pierceCount = (int)upgradeManager.GetUpgradeValue(UpgradeID.PiercingBullet);
            int ricochetCount = (int)upgradeManager.GetUpgradeValue(UpgradeID.RicochetBullet);

            bullet.SetActive(true);
            
            // YENİ: Mermiye artık hem delme hem sekme değerini veriyoruz
            bullet.GetComponent<Bullet>().Fire(fireDirection, pierceCount, ricochetCount);
        }
    }

    GameObject GetPooledBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeInHierarchy) return bulletPool[i];
        }
        return null; 
    }
}