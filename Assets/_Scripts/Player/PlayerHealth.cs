using UnityEngine;
using UnityEngine.InputSystem; // playerIndex için Input sistemi kütüphanesi eklendi

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [HideInInspector]
    public int myPlayerIndex; // 0 = Player 1, 1 = Player 2

    private PlayerController playerController;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();

        // OYUNCUNUN KÝMLÝÐÝNÝ TESPÝT ET (P1 mi P2 mi?)
        PlayerInput pi = GetComponent<PlayerInput>();
        if (pi != null)
        {
            myPlayerIndex = pi.playerIndex;
        }

        // UI'a "Ben doðdum, caným ful" mesajýný kendi kimliðimle yolla
        GameEvents.TriggerHealthChanged(myPlayerIndex, currentHealth, maxHealth);
    }

    // Mermi doðrudan bu fonksiyonu çaðýracak (Radyo dinlemesini sildik)
    // NOT: Public yapmak zorundayýz ki mermi dýþarýdan ulaþabilsin
    public void TakeDamage(int damageAmount)
    {
        if (playerController != null && playerController.isDashing)
        {
            Debug.Log($"Player {myPlayerIndex + 1} Dodge attý, hasardan kaçýnýldý! Kamera sallanmayacak.");
            return;
        }

        // --- EÐER BURAYA GELEBÝLDÝYSEK DASH ATMIYORUZ DEMEKTÝR ---
        currentHealth -= damageAmount;

        // KAMERAYA ONAY VER: Kendi kimliðini de gönder
        GameEvents.TriggerPlayerDamaged(myPlayerIndex, damageAmount);

        // UI (CAN BARI) GÜNCELLE: "Benim (P1/P2) caným deðiþti" diye anons et
        GameEvents.TriggerHealthChanged(myPlayerIndex, currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"Player {myPlayerIndex + 1} Öldü! Game Over.");
        GameEvents.TriggerPlayerDied(myPlayerIndex);

        // Karakteri yok etmeyelim, sadece görünmez yapalým
        gameObject.SetActive(false);
    }
}