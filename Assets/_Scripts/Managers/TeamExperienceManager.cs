using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI; 

public class TeamExperienceManager : MonoBehaviour
{
    public static TeamExperienceManager Instance; 

    [Header("Takım Seviye Sistemi")]
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    [Header("Arayüz Bağlantıları")]
    public Slider teamExpBar; 

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateExpBar(); 
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        UpdateExpBar();

        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    void UpdateExpBar()
    {
        if (teamExpBar != null)
        {
            teamExpBar.maxValue = expToNextLevel;
            teamExpBar.value = currentExp;
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.5f);
        UpdateExpBar();

        // LevelUpManager'a sahnede kaç kişi olduğunu söyleyerek ekranı açtırıyoruz
        int playerCount = PlayerInputManager.instance != null ? PlayerInputManager.instance.playerCount : 1;
        LevelUpManager.Instance.ShowDynamicLevelUpScreen(playerCount);
    }
}