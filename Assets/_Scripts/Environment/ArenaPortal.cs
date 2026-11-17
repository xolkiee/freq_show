using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArenaPortal : MonoBehaviour
{
    [Header("Hedef Arena Ayarları")]
    public Transform arenaSpawnPoint; // Arena 1'in başlangıç noktası

    [Tooltip("Arena 1'in etrafını saran BoxCollider2D objesini buraya sürükleyin.")]
    public BoxCollider2D arenaBounds; // YENİ: Kameranın dışarı çıkmasını engelleyecek sınır

    public GameObject lobbyObject;    // Kapatılacak lobi objesi (İsteğe bağlı)
    public GameObject arenaObject;    // Açılacak Arena 1 objesi (İsteğe bağlı)

    private List<GameObject> playersInPortal = new List<GameObject>();
    private CharacterSelectManager selectManager;

    void Start()
    {
        selectManager = FindFirstObjectByType<CharacterSelectManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playersInPortal.Contains(other.gameObject))
        {
            playersInPortal.Add(other.gameObject);
            CheckIfAllPlayersReady();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playersInPortal.Contains(other.gameObject))
        {
            playersInPortal.Remove(other.gameObject);
        }
    }

    void CheckIfAllPlayersReady()
    {
        // En az 1 oyuncu doğmuşsa ve sahnedeki TÜM oyuncular portalın içine girmişse
        if (selectManager != null && selectManager.currentPlayers > 0 && playersInPortal.Count >= selectManager.currentPlayers)
        {
            TeleportToArena();
        }
    }

    void TeleportToArena()
    {
        // 1. Yeni arenayı aç
        if (arenaObject != null) arenaObject.SetActive(true);

        // 2. Tüm oyuncuları yeni arenanın başlangıç noktasına ışınla
        int offset = 0;
        foreach (var player in PlayerInput.all)
        {
            if (arenaSpawnPoint != null)
            {
                player.transform.position = arenaSpawnPoint.position + new Vector3(offset * 1.5f, 0, 0);
                offset++;
            }
        }

        // 3. --- KAMERA SİHRİ --- (YENİ EKLENEN KISIM)
        if (DynamicCamera.Instance != null && arenaBounds != null)
        {
            // Kameraya yeni devasa arenanın sınırlarını ver
            DynamicCamera.Instance.SetNewBounds(arenaBounds);

            // Kamerayı lobi ile arena arasında kaydırmadan anında yeni hedefine oturt
            DynamicCamera.Instance.SnapToMidpoint();
        }

        // 4. Lobi seçimlerini kilitle ve lobiyi kapat
        selectManager.LockLobbyAndStart();
        if (lobbyObject != null) lobbyObject.SetActive(false);

        // 5. SAVAŞI VE MÜZİĞİ BAŞLAT!
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);
        }

        Debug.Log("Tüm oyuncular Arena 1'e ışınlandı ve kamera yeni sınırlara kilitlendi!");
    }
}