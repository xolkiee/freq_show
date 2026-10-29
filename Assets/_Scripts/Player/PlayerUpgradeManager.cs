using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveUpgrade 
{
    public UpgradeData data;
    public int currentLevel;

    public float GetCurrentValue() 
    {
        return data.baseValue + ((currentLevel - 1) * data.valuePerLevel);
    }
}

public class PlayerUpgradeManager : MonoBehaviour
{
    [Header("Karaktere Özel Yetenek Havuzu")]
    public List<UpgradeData> availableUpgrades = new List<UpgradeData>();

    // Mermi, Temas ve Dash grupları
    public Dictionary<UpgradeGroup, ActiveUpgrade> exclusiveUpgrades = new Dictionary<UpgradeGroup, ActiveUpgrade>();
    
    // Standart/Diğer güçler (Stackable)
    public List<ActiveUpgrade> otherUpgrades = new List<ActiveUpgrade>();

    public void AddOrUpdateUpgrade(UpgradeData newUpgrade)
    {
        if (newUpgrade.group == UpgradeGroup.Other)
        {
            otherUpgrades.Add(new ActiveUpgrade { data = newUpgrade, currentLevel = 1 });
            ApplyOtherBuffs(newUpgrade);
            return;
        }

        if (exclusiveUpgrades.ContainsKey(newUpgrade.group))
        {
            if (exclusiveUpgrades[newUpgrade.group].data.id == newUpgrade.id)
            {
                if (exclusiveUpgrades[newUpgrade.group].currentLevel < newUpgrade.maxLevel)
                {
                    exclusiveUpgrades[newUpgrade.group].currentLevel++;
                    Debug.Log(newUpgrade.upgradeName + " Seviye Atladı! Yeni Seviye: " + exclusiveUpgrades[newUpgrade.group].currentLevel);
                }
            }
            else
            {
                exclusiveUpgrades[newUpgrade.group] = new ActiveUpgrade { data = newUpgrade, currentLevel = 1 };
                Debug.Log("Yetenek Değişti! Yeni Güç: " + newUpgrade.upgradeName);
            }
        }
        else
        {
            exclusiveUpgrades.Add(newUpgrade.group, new ActiveUpgrade { data = newUpgrade, currentLevel = 1 });
            Debug.Log("Yeni Yetenek Eklendi: " + newUpgrade.upgradeName);
        }
    }

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
                    shooting.fireRate -= upgrade.baseValue; 
                    if (shooting.fireRate < 0.05f) shooting.fireRate = 0.05f; 
                    Debug.Log("Daha seri ateş ediliyor! Yeni FireRate: " + shooting.fireRate);
                }
                break;

            case UpgradeID.MaxHealth:
                PlayerHealth health = GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.maxHealth += (int)upgrade.baseValue;
                    health.currentHealth += (int)upgrade.baseValue; 
                    GameEvents.TriggerHealthChanged(health.myPlayerIndex, health.currentHealth, health.maxHealth);
                    Debug.Log("Maksimum Can Arttı!");
                }
                break;

            case UpgradeID.DamageIncrease:
                PlayerShooting shootingDamage = GetComponent<PlayerShooting>();
                if (shootingDamage != null)
                {
                    shootingDamage.extraDamage += (int)upgrade.baseValue;
                    Debug.Log($"Hasar Arttı! Ek Hasar: +{shootingDamage.extraDamage}");
                }
                break;

            case UpgradeID.BulletSize:
                PlayerShooting shootingSize = GetComponent<PlayerShooting>();
                if (shootingSize != null)
                {
                    shootingSize.bulletSizeMultiplier += upgrade.baseValue; 
                    Debug.Log($"Mermi Boyutu Arttı! Yeni Çarpan: {shootingSize.bulletSizeMultiplier}");
                }
                break;
        }
    }

    // Silah/Dash kodlarının mekanik değeri (Örn: Hasar, Hız) okuması için
    public float GetUpgradeValue(UpgradeID id)
    {
        foreach (var upgrade in exclusiveUpgrades.Values)
        {
            if (upgrade.data.id == id) return upgrade.GetCurrentValue();
        }
        return 0f; 
    }

    // YENİ / DÜZELTME: Yeteneğin kaçıncı seviyede olduğunu doğrudan INT olarak döner (MaxLevel kontrolü için)
    public int GetUpgradeLevel(UpgradeID id)
    {
        // 1. Exclusive grupta ara
        foreach (var upgrade in exclusiveUpgrades.Values)
        {
            if (upgrade.data.id == id) return upgrade.currentLevel;
        }

        // 2. Other (Pasif) grupta kaç defa alındığını say
        int count = 0;
        foreach (var upgrade in otherUpgrades)
        {
            if (upgrade.data.id == id) count++;
        }
        return count;
    }
}