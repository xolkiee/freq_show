using UnityEngine;

public enum UpgradeGroup { Bullet1, Bullet2, Dash, Other }
public enum UpgradeID { // Hızlı Karakter (Agile)
    PiercingBullet, RicochetBullet, FireDash, CuttingWind, MoveSpeed, FireRate, 
    
    // Güçlü Karakter (Heavy)
    Shrapnel, Cannonball, MagmaPool, SeismicShock, GroundSmash, Shield, BulletSize, DamageIncrease,
    
    // Ortak (Shared)
    MaxHealth}

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