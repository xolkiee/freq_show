using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Bu UI Kimin Canýný Gösterecek?")]
    [Tooltip("Player 1 için 0, Player 2 için 1 yazýn")]
    public int targetPlayerIndex = 0;

    private Slider healthSlider;

    void Awake()
    {
        healthSlider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        // Radyoyu dinlemeye baþla
        GameEvents.OnHealthChanged += UpdateHealthBar;
    }

    void OnDisable()
    {
        // Kapanýrken radyodan çýk
        GameEvents.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int incomingPlayerIndex, int currentHealth, int maxHealth)
    {
        // Gelen anons benim takip ettiðim oyuncuya (0 veya 1) mý ait?
        if (incomingPlayerIndex == targetPlayerIndex)
        {
            // Eþleþiyorsa slider deðerlerini güncelle
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
}