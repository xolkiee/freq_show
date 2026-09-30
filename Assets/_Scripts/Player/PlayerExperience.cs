using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Header("Seviye Sistemi")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100; // İlk seviyeyi geçmek için 100 exp (10 düşman) gereksin
    public LevelUpManager levelUpManager;

    // Yerdeki EXP taşı oyuncunun (Player_1) Trigger'ına değdiğinde çalışır
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ExpGem"))
        {
            // Taştaki değeri al
            ExpGem gem = col.GetComponent<ExpGem>();
            if (gem != null)
            {
                AddExp(gem.expValue);
                Destroy(col.gameObject); // Topladığımız taşı sahneden sil
            }
        }
    }

    void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log("EXP Toplandı! Mevcut EXP: " + currentExp + "/" + expToNextLevel);

        // Seviye atlama kontrolü
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel; // Fazlalık EXP'yi bir sonraki seviyeye devret
        
        // Her seviyede bir sonraki seviyenin zorluğunu 1.5 kat arttır
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.5f); 

        Debug.Log("LEVEL UP! Yeni Seviyen: " + currentLevel + " | Sonraki hedef: " + expToNextLevel);
        
        if(levelUpManager != null)
        {
            levelUpManager.ShowLevelUpScreen();
        }
    }
}