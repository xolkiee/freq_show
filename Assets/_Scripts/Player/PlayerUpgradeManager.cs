using System.Collections.Generic;
using UnityEngine;

// O an aktif olan yeteneğin verisini ve seviyesini tutan küçük bir sınıf
public class ActiveUpgrade 
{
    public UpgradeData data;
    public int currentLevel;

    public float GetCurrentValue() 
    {
        // 1. Seviyede baseValue, 2. seviyede baseValue + (1 * valuePerLevel) verir
        return data.baseValue + ((currentLevel - 1) * data.valuePerLevel);
    }
}

public class PlayerUpgradeManager : MonoBehaviour
{
    // Mermi, Temas ve Dash grupları sadece 1 tane barınabileceği için Sözlük (Dictionary) kullanıyoruz
    public Dictionary<UpgradeGroup, ActiveUpgrade> exclusiveUpgrades = new Dictionary<UpgradeGroup, ActiveUpgrade>();
    
    // Standart/Diğer güçler üst üste binebileceği için Liste kullanıyoruz
    public List<ActiveUpgrade> otherUpgrades = new List<ActiveUpgrade>();

    public void AddOrUpdateUpgrade(UpgradeData newUpgrade)
    {
        if (newUpgrade.group == UpgradeGroup.Other)
        {
            // Other (Standart) yetenekse direkt ekle
            otherUpgrades.Add(new ActiveUpgrade { data = newUpgrade, currentLevel = 1 });
            ApplyOtherBuffs(newUpgrade);
            return;
        }

        // Mermi, Temas veya Dash ise:
        if (exclusiveUpgrades.ContainsKey(newUpgrade.group))
        {
            // Sende bu gruptan bir yetenek var. Aynı yetenek mi?
            if (exclusiveUpgrades[newUpgrade.group].data.id == newUpgrade.id)
            {
                // Aynı yetenek -> Seviye Atlat!
                if (exclusiveUpgrades[newUpgrade.group].currentLevel < newUpgrade.maxLevel)
                {
                    exclusiveUpgrades[newUpgrade.group].currentLevel++;
                    Debug.Log(newUpgrade.upgradeName + " Seviye Atladı! Yeni Seviye: " + exclusiveUpgrades[newUpgrade.group].currentLevel);
                }
            }
            else
            {
                // Farklı yetenek -> Eskisini unut, yenisini 1. seviyeden başlat!
                exclusiveUpgrades[newUpgrade.group] = new ActiveUpgrade { data = newUpgrade, currentLevel = 1 };
                Debug.Log("Yetenek Değişti! Yeni Güç: " + newUpgrade.upgradeName);
            }
        }
        else
        {
            // Bu gruptan hiç yeteneğin yoktu, ilk kez ekleniyor
            exclusiveUpgrades.Add(newUpgrade.group, new ActiveUpgrade { data = newUpgrade, currentLevel = 1 });
            Debug.Log("Yeni Yetenek Eklendi: " + newUpgrade.upgradeName);
        }
    }

    // "Other" grubuna ait güçlendirmeleri (Can artışı, atış hızı vb.) anında uygulayacağımız yer
    // "Other" grubuna ait güçlendirmeleri anında uygulayacağımız yer
    void ApplyOtherBuffs(UpgradeData upgrade)
    {
        switch (upgrade.id)
        {
            case UpgradeID.MoveSpeed:
                PlayerController controller = GetComponent<PlayerController>();
                if (controller != null)
                {
                    controller.moveSpeed += upgrade.baseValue;
                    Debug.Log("Hareket hızı arttı! Yeni Hız: " + controller.moveSpeed);
                }
                break;

            case UpgradeID.FireRate:
                PlayerShooting shooting = GetComponent<PlayerShooting>();
                if (shooting != null)
                {
                    // Atış hızını (bekleme süresini) azaltıyoruz ki daha seri ateş etsin
                    shooting.fireRate -= upgrade.baseValue; 
                    
                    // Bekleme süresi eksiye düşmesin diye bir sınır (0.05 saniye) koyuyoruz
                    if (shooting.fireRate < 0.05f) shooting.fireRate = 0.05f; 
                    
                    Debug.Log("Daha seri ateş ediliyor! Yeni FireRate: " + shooting.fireRate);
                }
                break;

            case UpgradeID.MaxHealth:
               
                PlayerHealth health = GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.maxHealth += (int)upgrade.baseValue;
                    health.currentHealth += (int)upgrade.baseValue; // Canı artınca mevcut canı da dolsun
                    // Can barı UI'ına "Canım değişti, çubuğu uzat ve doldur" mesajı yolluyoruz
                    GameEvents.TriggerHealthChanged(health.myPlayerIndex, health.currentHealth, health.maxHealth);

                    Debug.Log("Maksimum Can Arttı!");
                }
                break;
        }
    }
    // Dışarıdan (Silah kodundan) bir yeteneğin o anki değerini okumak için
    public float GetUpgradeValue(UpgradeID id)
    {
        foreach (var upgrade in exclusiveUpgrades.Values)
        {
            if (upgrade.data.id == id) return upgrade.GetCurrentValue();
        }
        return 0f; // Yetenek yoksa 0 döner
    }
}