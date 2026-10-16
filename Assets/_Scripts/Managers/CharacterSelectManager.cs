using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Karakter Prefablarý")]
    public GameObject heavyPrefab;
    public GameObject agilePrefab;

    [Header("Lobi Doðma Noktasý")]
    public Transform lobbySpawnPoint; // Karakterlerin lobide doðacaðý yer

    [Header("UI ve Yönetim")]
    public GameObject selectionUI;
    private PlayerInputManager inputManager;
    public int currentPlayers = 0;

    private bool isGameStarted = false;

    void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        Time.timeScale = 1f; // Lobide rahatça yürüyüp ateþ edebilmeleri için zaman akýyor!
    }

    void Update()
    {
        // Oyun baþladýysa veya maksimum oyuncu sayýsýna (2) ulaþýldýysa yeni giriþ alma
        if (isGameStarted || currentPlayers >= 2) return;

        // --- KLAVYE SEÇÝMLERÝ ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                SpawnPlayer(heavyPrefab, "KeyboardMouse", Keyboard.current);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
                SpawnPlayer(agilePrefab, "KeyboardMouse", Keyboard.current);
        }

        // --- GAMEPAD SEÇÝMLERÝ ---
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.dpad.left.wasPressedThisFrame)
                SpawnPlayer(heavyPrefab, "Gamepad", gamepad);
            else if (gamepad.dpad.right.wasPressedThisFrame)
                SpawnPlayer(agilePrefab, "Gamepad", gamepad);
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
        PlayerInput newPlayer = inputManager.JoinPlayer(-1, -1, scheme, device);

        // Oyuncuyu lobinin ortasýna yerleþtir
        if (lobbySpawnPoint != null && newPlayer != null)
        {
            newPlayer.transform.position = lobbySpawnPoint.position + new Vector3(currentPlayers * 1.5f, 0, 0);
        }

        currentPlayers++;
        GameManager.Instance.RegisterPlayer(); // GameManager'a oyuncuyu kaydet
    }

    // Portal tarafýndan çaðrýlacak fonksiyon
    public void LockLobbyAndStart()
    {
        isGameStarted = true;
        if (selectionUI != null) selectionUI.SetActive(false);
    }
}