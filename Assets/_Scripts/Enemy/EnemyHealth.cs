using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    // YENİ: Düşmanın yere bırakacağı taşın şablonu
    public GameObject expGemPrefab; 

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // YENİ: Ölmeden hemen önce kendi bulunduğu kordinata EXP taşını yarat!
        if (expGemPrefab != null)
        {
            Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); 
    }
}