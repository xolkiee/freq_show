using UnityEngine;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    // YENİ: Sahnedeki her objenin bu koda anında ulaşabilmesi için statik referans
    public static LevelUpManager Instance;

    public GameObject levelUpPanel;
    public TMP_Text[] buttonTexts; 

    // YENİ: Hangi oyuncunun level atladığını hafızada tutacağız
    private GameObject levelingPlayer;

    void Awake()
    {
        // Oyun başladığında kendini merkeze kaydet
        Instance = this;
    }

    void Start()
    {
        levelUpPanel.SetActive(false);
    }

    // Karakter level atladığında kendini (GameObject) buraya parametre olarak gönderecek
    public void ShowLevelUpScreen(GameObject playerWhoLeveledUp)
    {
        levelingPlayer = playerWhoLeveledUp; // Level atlayan karakteri kaydet
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; 
        
        // Klasik tek kişilik rastgele buff kodunu buraya ekleyebilirsin
    }

    // Butona tıklandığında çalışacak fonksiyon
    public void ChooseBuff(int buffID)
    {
        if (levelingPlayer != null)
        {
            // Güçlendirmeyi SADECE level atlayan karaktere (levelingPlayer) uygula
            switch (buffID)
            {
                case 0:
                    levelingPlayer.GetComponent<PlayerController>().moveSpeed += 1.5f;
                    Debug.Log("Hız buff'ı uygulandı!");
                    break;
                case 1:
                    levelingPlayer.GetComponent<PlayerShooting>().fireRate *= 0.8f;
                    Debug.Log("Atış hızı buff'ı uygulandı!");
                    break;
                case 2:
                    // levelingPlayer.GetComponent<PlayerHealth>().maxHealth += 1;
                    Debug.Log("Can buff'ı uygulandı!");
                    break;
            }
        }

        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}