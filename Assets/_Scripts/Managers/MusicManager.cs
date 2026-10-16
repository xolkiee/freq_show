using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Ses Kaynaklarý")]
    public AudioSource musicSource; // FFT scriptinin dinleyeceði ana kaynak

    [Header("Müzik Parçalarý")]
    public AudioClip lobbyMusic;
    public AudioClip arena1Music;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Oyun açýldýðýnda direkt ambient lobi müziðini baþlat
        PlayMusic(lobbyMusic);
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (musicSource.clip == newClip) return;

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();
    }

    public void SwitchToArenaMusic()
    {
        PlayMusic(arena1Music);
    }
}