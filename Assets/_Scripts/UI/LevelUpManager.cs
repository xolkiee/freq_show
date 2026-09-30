using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance; // Merkezden çağrılabilmesi için eklendi
    public static bool isGamePausedForLevelUp = false; 

    [Header("Ana Paneller")]
    public GameObject levelUpPanel;
    public GameObject singlePlayerContainer; 
    public GameObject coopContainer; 

    [Header("Buton Yazıları")]
    public TMP_Text[] singleButtonTexts; // Tek oyunculu (Ortadaki) 3 buton
    public TMP_Text[] p1ButtonTexts;     // Co-op (Soldaki) 3 buton
    public TMP_Text[] p2ButtonTexts;     // Co-op (Sağdaki) 3 buton

    private int activePlayerCount;
    private bool p1HasChosen = false;
    private bool p2HasChosen = false;
    private bool singlePlayerHasChosen = false;

    private int[] currentChoices = new int[3]; // Tek oyuncu için havuz
    private int[] p1CurrentChoices = new int[3];
    private int[] p2CurrentChoices = new int[3];

    private int p1SelectedBuffID = -1;
    private int p2SelectedBuffID = -1;

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

        // Seçim kilitlerini sıfırla
        p1HasChosen = false;
        p2HasChosen = false;
        singlePlayerHasChosen = false;

        if (playerCount == 1)
        {
            singlePlayerContainer.SetActive(true);
            coopContainer.SetActive(false);
            GenerateBuffsForSinglePlayer();
        }
        else
        {
            singlePlayerContainer.SetActive(false);
            coopContainer.SetActive(true);
            GenerateBuffsForP1();
            GenerateBuffsForP2();
        }
    }

    // --- TEK OYUNCU İÇİN (SOLO) ---
    void GenerateBuffsForSinglePlayer()
    {
        // NOT: İleride Furkan'ın CharacterData (SO) sistemini buraya bağlayacağız
        // Şimdilik test amaçlı rastgele havuz oluşturuyoruz
        List<int> pool = new List<int> { 0, 1, 2, 10, 11, 12 }; 
        for (int i = 0; i < 3; i++)
        {
            int r = Random.Range(0, pool.Count);
            currentChoices[i] = pool[r];
            pool.RemoveAt(r);
            
            singleButtonTexts[i].text = "Yetenek ID: " + currentChoices[i];
        }
    }

    public void SinglePlayerSelectBuff(int index)
    {
        if (singlePlayerHasChosen) return;
        
        p1SelectedBuffID = currentChoices[index];
        singlePlayerHasChosen = true;
        singleButtonTexts[index].text = "SEÇİLDİ!";
        
        CheckReady(); // 1 kişi olduğu için direkt oyunu devam ettirir
    }

    // --- CO-OP İÇİN (2 OYUNCU) ---
    void GenerateBuffsForP1()
    {
        List<int> pool = new List<int> { 0, 1, 2 }; // Örn: Sadece Heavy yetenekleri
        for (int i = 0; i < 3; i++)
        {
            int r = Random.Range(0, pool.Count);
            p1CurrentChoices[i] = pool[r];
            pool.RemoveAt(r);
            p1ButtonTexts[i].text = "P1 Yetenek: " + p1CurrentChoices[i];
        }
    }

    void GenerateBuffsForP2()
    {
        List<int> pool = new List<int> { 10, 11, 12 }; // Örn: Sadece Agile yetenekleri
        for (int i = 0; i < 3; i++)
        {
            int r = Random.Range(0, pool.Count);
            p2CurrentChoices[i] = pool[r];
            pool.RemoveAt(r);
            p2ButtonTexts[i].text = "P2 Yetenek: " + p2CurrentChoices[i];
        }
    }

    public void P1SelectBuff(int index)
    {
        if (p1HasChosen) return;
        p1SelectedBuffID = p1CurrentChoices[index];
        p1HasChosen = true;
        p1ButtonTexts[index].text = "BEKLENİYOR..";
        CheckReady();
    }

    public void P2SelectBuff(int index)
    {
        if (p2HasChosen) return;
        p2SelectedBuffID = p2CurrentChoices[index];
        p2HasChosen = true;
        p2ButtonTexts[index].text = "BEKLENİYOR..";
        CheckReady();
    }

    // --- ORTAK ONAY VE OYUNA DÖNÜŞ ---
    void CheckReady()
    {
        if (activePlayerCount == 1 && singlePlayerHasChosen)
        {
            ApplyBuffs();
            ResumeGame();
        }
        else if (activePlayerCount == 2 && p1HasChosen && p2HasChosen)
        {
            ApplyBuffs();
            ResumeGame();
        }
    }

    void ApplyBuffs()
    {
        Debug.Log("P1'e verilecek Buff ID: " + p1SelectedBuffID);
        if(activePlayerCount == 2) Debug.Log("P2'ye verilecek Buff ID: " + p2SelectedBuffID);
    }

    void ResumeGame()
    {
        levelUpPanel.SetActive(false); 
        isGamePausedForLevelUp = false; 
        Time.timeScale = 1f; 
    }
}