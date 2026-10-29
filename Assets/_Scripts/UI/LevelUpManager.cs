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

    private int activePlayerCount;
    private bool p1HasChosen = false;
    private bool p2HasChosen = false;
    
    private UpgradeData[] p1Choices = new UpgradeData[3];
    private UpgradeData[] p2Choices = new UpgradeData[3];

    private PlayerUpgradeManager p1Manager;
    private PlayerUpgradeManager p2Manager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
    }

    public void ShowDynamicLevelUpScreen(int playerCount)
    {
        isGamePausedForLevelUp = true; 
        Time.timeScale = 0f; 
        levelUpPanel.SetActive(true);
        activePlayerCount = playerCount;

        p1HasChosen = false;
        p2HasChosen = false;

        FindPlayerManagers();

        if (activePlayerCount <= 1)
        {
            singlePlayerContainer.SetActive(true);
            coopContainer.SetActive(false);

            if (p1Manager != null)
            {
                p1Choices = GetRandomThreeUpgrades(p1Manager);
                DisplayUpgrades(p1Choices, singleButtonTexts);
            }
        }
        else
        {
            singlePlayerContainer.SetActive(false);
            coopContainer.SetActive(true);

            if (p1Manager != null)
            {
                p1Choices = GetRandomThreeUpgrades(p1Manager);
                DisplayUpgrades(p1Choices, p1ButtonTexts);
            }

            if (p2Manager != null)
            {
                p2Choices = GetRandomThreeUpgrades(p2Manager);
                DisplayUpgrades(p2Choices, p2ButtonTexts);
            }
        }
    }

    void FindPlayerManagers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        p1Manager = null;
        p2Manager = null;

        if (players.Length > 0) p1Manager = players[0].GetComponent<PlayerUpgradeManager>();
        if (players.Length > 1) p2Manager = players[1].GetComponent<PlayerUpgradeManager>();
    }

    UpgradeData[] GetRandomThreeUpgrades(PlayerUpgradeManager manager)
    {
        UpgradeData[] result = new UpgradeData[3];

        if (manager == null || manager.availableUpgrades == null) 
            return result;
        
        List<UpgradeData> validPool = new List<UpgradeData>();
        foreach (UpgradeData upgrade in manager.availableUpgrades)
        {
            if (upgrade == null) continue;

            // DÜZELTME: Artık GetUpgradeLevel() fonksiyonunu kullanarak gerçek seviye sayısını alıyoruz
            int currentLvl = manager.GetUpgradeLevel(upgrade.id);
            if (currentLvl < upgrade.maxLevel)
            {
                validPool.Add(upgrade);
            }
        }

        List<UpgradeData> tempPool = new List<UpgradeData>(validPool);
        for (int i = 0; i < 3; i++)
        {
            if (tempPool.Count > 0)
            {
                int randomIndex = Random.Range(0, tempPool.Count);
                result[i] = tempPool[randomIndex];
                tempPool.RemoveAt(randomIndex); 
            }
            else if (validPool.Count > 0)
            {
                result[i] = validPool[0];
            }
        }

        return result;
    }

    void DisplayUpgrades(UpgradeData[] choices, TMP_Text[] buttonTexts)
    {
        for (int i = 0; i < 3; i++)
        {
            if (choices[i] != null && i < buttonTexts.Length)
            {
                buttonTexts[i].text = $"<b>{choices[i].upgradeName}</b>\n<size=80%>{choices[i].description}</size>";
            }
        }
    }

    public void SinglePlayerSelectBuff(int index)
    {
        if (p1HasChosen) return;

        if (p1Manager != null && p1Choices[index] != null)
        {
            p1Manager.AddOrUpdateUpgrade(p1Choices[index]);
            p1HasChosen = true;
            singleButtonTexts[index].text = "SEÇİLDİ!";
            CheckReady();
        }
    }

    public void SelectP1Buff(int index)
    {
        if (p1HasChosen) return;

        if (p1Manager != null && p1Choices[index] != null)
        {
            p1Manager.AddOrUpdateUpgrade(p1Choices[index]);
            p1HasChosen = true;
            p1ButtonTexts[index].text = "HAZIR!";
            CheckReady();
        }
    }

    public void SelectP2Buff(int index)
    {
        if (p2HasChosen) return;

        if (p2Manager != null && p2Choices[index] != null)
        {
            p2Manager.AddOrUpdateUpgrade(p2Choices[index]);
            p2HasChosen = true;
            p2ButtonTexts[index].text = "HAZIR!";
            CheckReady();
        }
    }

    void CheckReady()
    {
        if (activePlayerCount <= 1)
        {
            if (p1HasChosen) ResumeGame();
        }
        else
        {
            if (p1HasChosen && p2HasChosen) ResumeGame();
        }
    }

    void ResumeGame()
    {
        levelUpPanel.SetActive(false); 
        isGamePausedForLevelUp = false; 
        Time.timeScale = 1f; 
    }
}