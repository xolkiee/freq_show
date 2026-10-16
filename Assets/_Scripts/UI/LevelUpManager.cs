using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance; 
    public static bool isGamePausedForLevelUp = false; 

    [Header("Ana Paneller")]
    public GameObject levelUpPanel;
    public GameObject singlePlayerContainer; 
    public GameObject coopContainer; 

    [Header("Buton Yazıları")]
    public TMP_Text[] singleButtonTexts; 
    public TMP_Text[] p1ButtonTexts;     
    public TMP_Text[] p2ButtonTexts;  

    [Header("Test Yetenek Havuzu")]
    public List<UpgradeData> testUpgrades; 

    private int activePlayerCount;
    private bool singlePlayerHasChosen = false;
    
    private UpgradeData[] singlePlayerChoices = new UpgradeData[3];
    private UpgradeData selectedUpgrade;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        levelUpPanel.SetActive(false);
    }

    public void ShowDynamicLevelUpScreen(int playerCount)
    {
        isGamePausedForLevelUp = true; 
        Time.timeScale = 0f; 
        levelUpPanel.SetActive(true);
        activePlayerCount = playerCount;

        singlePlayerHasChosen = false;

        singlePlayerContainer.SetActive(true);
        coopContainer.SetActive(false);
        GenerateBuffsForSinglePlayer();
    }

    void GenerateBuffsForSinglePlayer()
    {
        for (int i = 0; i < 3; i++)
        {
            if (testUpgrades.Count > 0)
            {
                // YENİ Zeka: Test listesinde kaç yetenek varsa sırayla butonlara diz. 
                // Eğer listede sadece 2 yetenek varsa ve 3. buton boş kalacaksa, çökmemesi için ilk yeteneği tekrar yazdır.
                int indexToUse = (i < testUpgrades.Count) ? i : 0;
                
                singlePlayerChoices[i] = testUpgrades[indexToUse]; 
                
                // ÇÖZÜM BURADA: "SEÇİLDİ" yazısını her menü açılışında ezip gerçek ismi yazdırıyoruz
                singleButtonTexts[i].text = singlePlayerChoices[i].upgradeName + "\n" + singlePlayerChoices[i].description;
            }
        }
    }

    public void SinglePlayerSelectBuff(int index)
    {
        if (singlePlayerHasChosen) return;
        
        selectedUpgrade = singlePlayerChoices[index];
        singlePlayerHasChosen = true;
        singleButtonTexts[index].text = "SEÇİLDİ!";
        
        CheckReady();
    }

    void CheckReady()
    {
        if (singlePlayerHasChosen)
        {
            ApplyBuffs();
            ResumeGame();
        }
    }

    void ApplyBuffs()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerUpgradeManager manager = player.GetComponent<PlayerUpgradeManager>();
            if (manager != null)
            {
                manager.AddOrUpdateUpgrade(selectedUpgrade);
            }
        }
    }

    void ResumeGame()
    {
        levelUpPanel.SetActive(false); 
        isGamePausedForLevelUp = false; 
        Time.timeScale = 1f; 
    }
}