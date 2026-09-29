using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3; // 3 mermide ölsün
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // İleride buraya vurulma anında beyazlama (Flash) efekti koyacağız
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // İleride patlama partikülü ve kan lekesi burada oluşacak
        Destroy(gameObject); 
    }
}