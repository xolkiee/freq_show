using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Karakter Prefablarý")]
    public GameObject heavyPrefab;
    public GameObject agilePrefab;

    [Header("UI ve Yönetim")]
    public GameObject selectionUI;
    private PlayerInputManager inputManager;
    private int currentPlayers = 0;

    private bool isGameStarted = false;

    void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();

        // --- ZAMANI VE SESLERÝ DONDUR ---
        Time.timeScale = 0f;         // Düþmanlarý, fizikleri ve animasyonlarý durdurur
        AudioListener.pause = true;  // Oyundaki tüm sesleri ve müzikleri susturur
    }

    void Update()
    {
        if (isGameStarted) return;

        if (currentPlayers >= 2)
        {
            StartGame();
            return;
        }

        // --- KLAVYE (PLAYER 1) SEÇÝMLERÝ ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                SpawnPlayer(heavyPrefab, "KeyboardMouse", Keyboard.current);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
                SpawnPlayer(agilePrefab, "KeyboardMouse", Keyboard.current);

            if (currentPlayers > 0 && Keyboard.current.enterKey.wasPressedThisFrame)
                StartGame();
        }

        // --- GAMEPAD (PLAYER 2) SEÇÝMLERÝ ---
        if (Gamepad.current != null)
        {
            if (Gamepad.current.dpad.left.wasPressedThisFrame)
                SpawnPlayer(heavyPrefab, "Gamepad", Gamepad.current);
            else if (Gamepad.current.dpad.right.wasPressedThisFrame)
                SpawnPlayer(agilePrefab, "Gamepad", Gamepad.current);

            if (currentPlayers > 0 && Gamepad.current.startButton.wasPressedThisFrame)
                StartGame();
        }
    }

    void SpawnPlayer(GameObject prefabToSpawn, string scheme, InputDevice device)
    {
        foreach (var player in PlayerInput.all)
        {
            foreach (var d in player.devices)
            {
                if (d == device) return;
            }
        }

        inputManager.playerPrefab = prefabToSpawn;
        inputManager.JoinPlayer(-1, -1, scheme, device);

        currentPlayers++;
    }

    void StartGame()
    {
        isGameStarted = true;
        if (selectionUI != null) selectionUI.SetActive(false);

        // --- ZAMANI VE SESLERÝ GERÝ AÇ ---
        Time.timeScale = 1f;          // Fizik ve oyun akýþý normale döner
        AudioListener.pause = false;  // Müzikler ve ses efektleri kaldýðý yerden patlar

        Debug.Log("Oyun Baþladý! Mevcut Oyuncu: " + currentPlayers);
    }
}