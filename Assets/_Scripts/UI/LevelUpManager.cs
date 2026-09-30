using UnityEngine;
using TMPro; // TextMeshPro buton yazıları için gerekli
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    [Header("Arayüz Bağlantıları")]
    public GameObject levelUpPanel;
    public TMP_Text[] buttonTexts; // 3 butonun içindeki Text objelerini buraya sürükleyeceğiz

    [Header("Oyuncu Bağlantıları")]
    public PlayerController playerController;
    public PlayerShooting playerShooting;

    // Ekrana gelen 3 rastgele yeteneğin arka plandaki ID'lerini tutacağız
    private int[] currentChoices = new int[3];

    void Start()
    {
        levelUpPanel.SetActive(false);
    }

    public void ShowLevelUpScreen()
    {
        GenerateRandomBuffs();
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    void GenerateRandomBuffs()
    {
        // Havuzdaki tüm yetenek ID'leri (0: Hız, 1: Atış Hızı, 2: Can, İleride 3,4,5 eklenecek)
        List<int> availableBuffs = new List<int> { 0, 1, 2 }; 

        for (int i = 0; i < 3; i++)
        {
            // Havuzdan rastgele bir yetenek seç
            int randomIndex = Random.Range(0, availableBuffs.Count);
            int selectedBuffID = availableBuffs[randomIndex];

            // Seçileni buton hafızasına al ve havuzdan çıkar (Aynı seçenek 2 kez gelmesin diye)
            currentChoices[i] = selectedBuffID;
            availableBuffs.RemoveAt(randomIndex);

            // Butonun yazısını seçilen yeteneğe göre güncelle
            UpdateUI(i, selectedBuffID);
        }
    }

    void UpdateUI(int buttonIndex, int buffID)
    {
        switch(buffID)
        {
            case 0: buttonTexts[buttonIndex].text = "Hareket Hızı +"; break;
            case 1: buttonTexts[buttonIndex].text = "Atış Hızı +"; break;
            case 2: buttonTexts[buttonIndex].text = "Maksimum Can +"; break;
        }
    }

    // Unity Inspector'dan gelen 0, 1, 2 değerleri artık BUTON SIRASINI belirtiyor
    public void ChooseBuff(int buttonIndex)
    {
        int selectedBuffID = currentChoices[buttonIndex]; // Tıklanan butona hangi yetenek atanmışsa onu bul

        switch (selectedBuffID)
        {
            case 0: playerController.moveSpeed += 1.5f; Debug.Log("Hız arttı!"); break;
            case 1: playerShooting.fireRate *= 0.8f; Debug.Log("Atış seri hale geldi!"); break;
            case 2: Debug.Log("Can artışı (Furkan bekleniyor)"); break;
        }

        ResumeGame();
    }

    void ResumeGame()
    {
        levelUpPanel.SetActive(false); 
        Time.timeScale = 1f; 
        
        RepelEnemies(); // Zaman akmaya başladığı an şok dalgası yarat!
    }

    void RepelEnemies()
    {
        // Sahnedeki tüm düşmanları bul
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector2 playerPos = playerController.transform.position;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                // Düşmanı itme yönünü hesapla (Oyuncudan düşmana doğru bir vektör)
                Vector2 repelDir = ((Vector2)enemy.transform.position - playerPos).normalized;
                
                // Düşmanı 4 birim geriye kaydır (Şok dalgası etkisi)
                enemy.transform.position = (Vector2)enemy.transform.position + repelDir * 4f;
            }
        }
    }
}