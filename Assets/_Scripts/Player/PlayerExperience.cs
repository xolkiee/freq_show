using UnityEngine;
using UnityEngine.InputSystem; // Furkan'ın yeni Input sistemi için gerekli

public class PlayerExperience : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    // Oyuncunun kimliği (0 veya 1)
    private int playerIndex;

    void Start()
    {
        // Furkan'ın PlayerInput bileşeninden bu karakterin kim olduğunu (Player 0 mı 1 mi) öğreniyoruz
        PlayerInput pInput = GetComponent<PlayerInput>();
        if (pInput != null)
        {
            playerIndex = pInput.playerIndex;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ExpGem"))
        {
            ExpGem gem = col.GetComponent<ExpGem>();
            if (gem != null)
            {
                AddExp(gem.expValue);
                Destroy(col.gameObject);
            }
        }
    }

    void AddExp(int amount)
    {
        currentExp += amount;
        // Kimin exp topladığını konsolda net görebilirsin
        Debug.Log("Player " + playerIndex + " EXP Topladı: " + currentExp + "/" + expToNextLevel);

        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.5f);

        Debug.Log("Player " + playerIndex + " LEVEL ATLADI!");

        // YENİ: Sahnedeki UI yöneticisini bul ve KENDİNİ (this.gameObject) oraya gönder
        if (LevelUpManager.Instance != null)
        {
            LevelUpManager.Instance.ShowLevelUpScreen(this.gameObject);
        }
    }
}