using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Lobby, Playing, LevelUp, GameOver }
    public GameState currentState;

    private int alivePlayers = 0;

    [Header("UI Panelleri")]
    public GameObject gameOverUI; // Game Over ekraný paneli

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        ChangeState(GameState.Lobby);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Lobby:
                Time.timeScale = 1f;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.SwitchToArenaMusic();
                }
                break;

            case GameState.LevelUp:
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;

                Debug.Log(">>> KOD GAME OVER DURUMUNA GÝRDÝ! Panel açýlýyor...");
                if (gameOverUI != null)
                {
                    gameOverUI.SetActive(true);
                }
                else
                {
                    Debug.LogWarning(">>> DIKKAT: gameOverUI boþ býrakýlmýþ! Ýsimle aranýyor...");
                    GameObject foundPanel = GameObject.Find("GameOverPanel");
                    if (foundPanel != null) foundPanel.SetActive(true);
                    else Debug.LogError(">>> HATA: Sahnede 'GameOverPanel' adýnda bir nesne bulunamadý!");
                }
                break;
        }
    }

    public void RegisterPlayer()
    {
        alivePlayers++;
        Debug.Log("Oyuncu kaydedildi. Toplam Canlý Oyuncu: " + alivePlayers);
    }

    public void PlayerDied()
    {
        alivePlayers--;
        Debug.Log("Bir oyuncu öldü! Kalan canlý oyuncu: " + alivePlayers);

        if (alivePlayers <= 0)
        {
            Debug.Log("Tüm oyuncular öldü, Game Over tetikleniyor!");
            ChangeState(GameState.GameOver);
        }
        else
        {
            Debug.Log("Hâlâ savaþan oyuncular var.");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}