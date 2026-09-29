using System;

public static class GameEvents
{
    // --- YENÝ EKLENEN KANAL (Düþmanlar önce bu kanala "Vurmak Ýstiyorum" diyecek) ---
    public static event Action<int> OnDamageAttempt;
    public static void TriggerDamageAttempt(int damageAmount)
    {
        OnDamageAttempt?.Invoke(damageAmount);
    }

    // --- ESKÝ KANALLAR (Sadece Kamera ve UI bunlarý dinleyecek) ---

    // Sadece hasar gerçekten alýndýðýnda (Dash atýlmýyorsa) tetiklenir
    public static event Action<int> OnPlayerDamaged;
    public static void TriggerPlayerDamaged(int damageAmount)
    {
        OnPlayerDamaged?.Invoke(damageAmount);
    }

    public static event Action OnPlayerDied;
    public static void TriggerPlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

    public static event Action<int, int> OnHealthChanged;
    public static void TriggerHealthChanged(int currentHealth, int maxHealth)
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}