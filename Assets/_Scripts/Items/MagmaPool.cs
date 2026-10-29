using UnityEngine;
using System.Collections.Generic;

public class MagmaPool : MonoBehaviour
{
    private int damage;
    private float tickRate = 0.4f; // kaç saniyede bir hasar verecek
    private float tickTimer = 0f;
    private HashSet<EnemyHealth> enemiesInPool = new HashSet<EnemyHealth>();

    public void Setup(int poolDamage, float duration)
    {
        damage = poolDamage;
        Destroy(gameObject, duration); // Belirtilen süre sonunda magmayı temizle
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= tickRate)
        {
            tickTimer = 0f;
            
            // Havuzun içindeki tüm düşmanlara periyodik hasar uygula
            enemiesInPool.RemoveWhere(e => e == null || !e.gameObject.activeInHierarchy);
            foreach (var enemy in enemiesInPool)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null && !enemiesInPool.Contains(health))
            {
                enemiesInPool.Add(health);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                enemiesInPool.Remove(health);
            }
        }
    }
}