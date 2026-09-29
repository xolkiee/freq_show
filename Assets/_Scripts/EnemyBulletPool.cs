using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletPool : MonoBehaviour
{
    // Singleton mimarisi: Herkes bu havuza anında ulaşabilsin diye!
    public static EnemyBulletPool Instance; 

    public GameObject enemyBulletPrefab;
    public int poolSize = 100; // Ekranda aynı anda 100 düşman mermisi olabilir
    private List<GameObject> pool;

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyBulletPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        return null; // Havuz bittiyse ateş edemezler
    }
}