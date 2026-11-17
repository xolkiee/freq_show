using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Karakter Prefablarý")]
    public GameObject heavyPrefab;
    public GameObject agilePrefab;

    [Header("Lobi Doðma Noktasý")]
    public Transform lobbySpawnPoint;

    [Header("UI ve Yönetim")]
    public GameObject selectionUI; // Ekranda çýkan "1: Heavy, 2: Agile seç, Enter'a bas" yazýsý
    private PlayerInputManager inputManager;
    public int currentPlayers = 0;
    private bool isGameStarted = false;

    // Her cihazýn þu an hangi karakteri önizlediðini ve hazýr olup olmadýðýný tutacaðýmýz yapýlar
    private Dictionary<InputDevice, int> deviceHoverIndex = new Dictionary<InputDevice, int>();
    private Dictionary<InputDevice, bool> deviceReadyStatus = new Dictionary<InputDevice, bool>();

    // Index 0: Heavy, Index 1: Agile
    private GameObject[] selectablePrefabs;

    void Start()
    {
        inputManager = GetComponent<PlayerInputManager>();
        Time.timeScale = 1f;
        selectablePrefabs = new GameObject[] { heavyPrefab, agilePrefab };
    }

    void Update()
    {
        if (isGameStarted || currentPlayers >= 2) return;

        // --- KLAVYE KONTROLLERÝ ---
        if (Keyboard.current != null)
        {
            HandleDeviceInput(Keyboard.current, Keyboard.current.digit1Key, Keyboard.current.digit2Key, Keyboard.current.enterKey, "KeyboardMouse");
        }

        // --- GAMEPAD KONTROLLERÝ ---
        foreach (var gamepad in Gamepad.all)
        {
            HandleDeviceInput(gamepad, gamepad.dpad.left, gamepad.dpad.right, gamepad.buttonSouth, "Gamepad"); // buttonSouth = Gamepad'deki A tuþu (Xbox) veya X (PlayStation)
        }
    }

    private void HandleDeviceInput(InputDevice device, UnityEngine.InputSystem.Controls.ButtonControl selectHeavyBtn, UnityEngine.InputSystem.Controls.ButtonControl selectAgileBtn, UnityEngine.InputSystem.Controls.ButtonControl readyBtn, string scheme)
    {
        // Eðer bu cihaz zaten oyuna katýlmýþ ve spawnlanmýþsa iþlemi geç
        if (IsDeviceAlreadySpawned(device)) return;

        // Cihazý sözlüklere ekle (Ýlk defa basýyorsa)
        if (!deviceHoverIndex.ContainsKey(device))
        {
            deviceHoverIndex[device] = 0; // Varsayýlan olarak Heavy seçili baþlasýn
            deviceReadyStatus[device] = false;
        }

        // EÐER OYUNCU HENÜZ HAZIR DEÐÝLSE KARAKTER DEÐÝÞTÝREBÝLÝR
        if (!deviceReadyStatus[device])
        {
            if (selectHeavyBtn.wasPressedThisFrame)
            {
                deviceHoverIndex[device] = 0;
                Debug.Log($"{device.name} cihazý HEAVY karakterini önizliyor.");
                UpdateUIPreview(device, 0); // UI'ý güncelle
            }
            else if (selectAgileBtn.wasPressedThisFrame)
            {
                deviceHoverIndex[device] = 1;
                Debug.Log($"{device.name} cihazý AGILE karakterini önizliyor.");
                UpdateUIPreview(device, 1); // UI'ý güncelle
            }

            // ONAYLAMA (READY) TUÞUNA BASILDIÐINDA
            if (readyBtn.wasPressedThisFrame)
            {
                deviceReadyStatus[device] = true;
                int selectedIndex = deviceHoverIndex[device];
                Debug.Log($"{device.name} cihazý HAZIR! Seçilen: {(selectedIndex == 0 ? "Heavy" : "Agile")}");

                // Karakteri artýk gerçekten oyuna sok (Instantiate et)
                SpawnPlayer(selectablePrefabs[selectedIndex], scheme, device);
            }
        }
    }

    private bool IsDeviceAlreadySpawned(InputDevice device)
    {
        foreach (var player in PlayerInput.all)
        {
            foreach (var d in player.devices)
            {
                if (d == device) return true;
            }
        }
        return false;
    }

    void SpawnPlayer(GameObject prefabToSpawn, string scheme, InputDevice device)
    {
        inputManager.playerPrefab = prefabToSpawn;
        PlayerInput newPlayer = inputManager.JoinPlayer(-1, -1, scheme, device);

        if (lobbySpawnPoint != null && newPlayer != null)
        {
            newPlayer.transform.position = lobbySpawnPoint.position + new Vector3(currentPlayers * 1.5f, 0, 0);

            // YENÝ: Oyuncunun UI modülünü sadece kendi cihazýna kilitle
            var uiModule = newPlayer.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            if (uiModule != null)
            {
                // Mevcut aksiyonlarý al ve bu cihaza özel hale getir
                uiModule.actionsAsset = newPlayer.actions;
            }
        }

        currentPlayers++;
        GameManager.Instance.RegisterPlayer();
    }

    private void UpdateUIPreview(InputDevice device, int prefabIndex)
    {
        // Burada UI üzerinde "1. Oyuncu þu an Heavy seçiyor" gibi görsel güncellemeler yapabilirsin.
        // Þimdilik sadece Console'a Log atýyoruz, ancak ileride ekrandaki ikonlarý deðiþtirmek için bu fonksiyonu doldurabilirsin.
    }

    public void LockLobbyAndStart()
    {
        isGameStarted = true;
        if (selectionUI != null) selectionUI.SetActive(false);
    }
}