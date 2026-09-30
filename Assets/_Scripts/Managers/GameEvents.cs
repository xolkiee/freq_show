using System;

public static class GameEvents
{
    // --- KÝMLÝKLENDÝRÝLMÝÞ (CO-OP) CAN VE HASAR KANALLARI ---

    // Sadece hasar gerçekten alýndýðýnda (Dash atýlmýyorsa) tetiklenir (Kamera titremesi vs. için)
    // Parametreler: playerIndex, damageAmount
    public static event Action<int, int> OnPlayerDamaged;
    public static void TriggerPlayerDamaged(int playerIndex, int damageAmount)
    {
        OnPlayerDamaged?.Invoke(playerIndex, damageAmount);
    }

    // Parametre: playerIndex
    public static event Action<int> OnPlayerDied;
    public static void TriggerPlayerDied(int playerIndex)
    {
        OnPlayerDied?.Invoke(playerIndex);
    }

    // UI (Can Barý) Güncellemesi
    // Parametreler: playerIndex, currentHealth, maxHealth
    public static event Action<int, int, int> OnHealthChanged;
    public static void TriggerHealthChanged(int playerIndex, int currentHealth, int maxHealth)
    {
        OnHealthChanged?.Invoke(playerIndex, currentHealth, maxHealth);
    }

    // --- MÜZÝK VE RÝTÝM KANALLARI (Deðiþmedi) ---
    public static event Action OnKickHit;
    public static void TriggerKickHit() => OnKickHit?.Invoke();

    public static event Action OnHiHatHit;
    public static void TriggerHiHatHit() => OnHiHatHit?.Invoke();

    public static event Action<float> OnSubIntensity;
    public static void TriggerSubIntensity(float intensity) => OnSubIntensity?.Invoke(intensity);
}