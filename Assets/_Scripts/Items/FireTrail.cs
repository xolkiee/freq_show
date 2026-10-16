using UnityEngine;
using System.Collections.Generic;

public class FireTrail : MonoBehaviour
{
    private int damage; 
    private List<GameObject> burnedEnemies = new List<GameObject>();

    // YENİ: Setup artık hem hasarı hem de ekranda kalma süresini (lifeTime) alıyor
    public void Setup(int burnDamage, float lifeTime)
    {
        damage = burnDamage;
        Destroy(gameObject, lifeTime); // Kendini dışarıdan gelen süreye göre yok et
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            if (!burnedEnemies.Contains(col.gameObject))
            {
                EnemyHealth enemy = col.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    burnedEnemies.Add(col.gameObject); 
                }
            }
        }
    }
}