using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private PlayerController playerController;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();

        // ARTIK OnPlayerDamaged DEÐÝL, OnDamageAttempt DÝNLÝYORUZ
        GameEvents.OnDamageAttempt += TakeDamage;

        GameEvents.TriggerHealthChanged(currentHealth, maxHealth);
    }

    void OnDestroy()
    {
        GameEvents.OnDamageAttempt -= TakeDamage;
    }

    private void TakeDamage(int damageAmount)
    {
        if (playerController != null && playerController.isDashing)
        {
            Debug.Log("Dodge atýldý, hasardan kaçýnýldý! Kamera sallanmayacak.");
            return; // Fonksiyondan çýk, aþaðýdaki hiçbir þey çalýþmasýn
        }

        // --- EÐER BURAYA GELEBÝLDÝYSEK DASH ATMIYORUZ DEMEKTÝR ---

        currentHealth -= damageAmount;

        // KAMERAYA ONAY VER: "Gerçekten hasar aldým, þimdi dünyayý sars!"
        GameEvents.TriggerPlayerDamaged(damageAmount);

        // UI (CAN BARI) GÜNCELLE
        GameEvents.TriggerHealthChanged(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Oyuncu Öldü! Game Over.");
        GameEvents.TriggerPlayerDied();

        // Karakteri þimdilik yok etmeyelim, sadece görünmez yapalým
        gameObject.SetActive(false);
    }
}