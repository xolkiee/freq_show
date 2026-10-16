using UnityEngine;

public enum UpgradeGroup { Bullet, Contact, Dash, Other }
public enum UpgradeID { PiercingBullet, RicochetBullet, FireDash, Shrapnel, FireRate, MaxHealth, MoveSpeed }

[CreateAssetMenu(fileName = "Yeni Yetenek", menuName = "Oyun/Yetenek")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;
    
    public UpgradeGroup group; // Hangi kategoriye ait?
    public UpgradeID id;       // Yeteneğin kesin kimliği
    
    [Header("Mekanik Değerler")]
    public float baseValue;       // 1. Seviyedeki değeri (Örn: 1 delici vuruş)
    public float valuePerLevel;   // Her seviye atladığında ne kadar artacak? (Örn: +1)
    public int maxLevel = 3;      // Maksimum çıkabileceği seviye
}