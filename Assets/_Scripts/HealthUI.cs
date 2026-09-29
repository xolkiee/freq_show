using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
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

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        // Slider deðerlerini güncelle
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}