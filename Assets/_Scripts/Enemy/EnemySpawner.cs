using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Temel Üretim Ayarları")]
    public GameObject enemyPrefab;
    public float baseSpawnInterval = 2f;
    public float minSpawnInterval = 0.3f;
    public float spawnOffset = 2f;

    [Header("Zorluk (Escalating Chaos) Ayarları")]
    public float difficultyIncreaseTimer = 10f;
    public float decreaseAmount = 0.15f;

    [Header("FFT (Müzik) Bağlantısı")]
    [Tooltip("Sub-bass şiddetine göre bu çarpan düşer, süre kısalır ve oyun hızlanır.")]
    public float fftMultiplier = 1f;

    private float nextSpawnTime;
    private float nextDifficultyIncrease;
    private Camera mainCam;

    void OnEnable()
    {
        // FURKAN'IN RADYOSUNA BAĞLANTI:
        GameEvents.OnKickHit += SpawnEnemyOutsideCamera; // Kick vurduğunda anında fırlat
        GameEvents.OnSubIntensity += AdjustMultiplier;   // Sub-bass şiddetine göre hızı katla
    }

    void OnDisable()
    {
        GameEvents.OnKickHit -= SpawnEnemyOutsideCamera;
        GameEvents.OnSubIntensity -= AdjustMultiplier;
    }

    void Start()
    {
        mainCam = Camera.main;
        nextSpawnTime = Time.time + GetCurrentSpawnInterval();
        nextDifficultyIncrease = Time.time + difficultyIncreaseTimer;
    }

    void Update()
    {
        // 1. DİNAMİK ZORLUK (Zamanla kendiliğinden hızlanma)
        if (Time.time >= nextDifficultyIncrease)
        {
            IncreaseDifficulty();
            nextDifficultyIncrease = Time.time + difficultyIncreaseTimer;
        }

        // 2. TEMEL ÜRETİM (Müzik dursa bile oyun boş kalmasın diye baseline)
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemyOutsideCamera();
            nextSpawnTime = Time.time + GetCurrentSpawnInterval();
        }
    }

    // FFT KANCASI 1: Sub-bass şiddeti arttıkça çarpan küçülür (0.2'ye kadar düşebilir)
    private void AdjustMultiplier(float intensity)
    {
        // Intensity 0 ile 100 arası geliyor (örneğin 20 şiddetinde vurduysa çarpan 0.8 olur)
        fftMultiplier = Mathf.Clamp(1f - (intensity / 100f), 0.2f, 1f);
    }

    void IncreaseDifficulty()
    {
        if (baseSpawnInterval > minSpawnInterval)
        {
            baseSpawnInterval -= decreaseAmount;
            baseSpawnInterval = Mathf.Max(baseSpawnInterval, minSpawnInterval);
        }
    }

    // FFT KANCASI 2: Üretim hızını belirleyen nihai matematik
    public float GetCurrentSpawnInterval()
    {
        float finalInterval = baseSpawnInterval * fftMultiplier;
        return Mathf.Max(finalInterval, 0.05f);
    }

    void SpawnEnemyOutsideCamera()
    {
        if (mainCam == null) return;

        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector2 camPos = mainCam.transform.position;

        int edge = Random.Range(0, 4);
        Vector2 spawnPosition = Vector2.zero;

        switch (edge)
        {
            case 0: spawnPosition = new Vector2(Random.Range(camPos.x - camWidth, camPos.x + camWidth), camPos.y + camHeight + spawnOffset); break;
            case 1: spawnPosition = new Vector2(Random.Range(camPos.x - camWidth, camPos.x + camWidth), camPos.y - camHeight - spawnOffset); break;
            case 2: spawnPosition = new Vector2(camPos.x + camWidth + spawnOffset, Random.Range(camPos.y - camHeight, camPos.y + camHeight)); break;
            case 3: spawnPosition = new Vector2(camPos.x - camWidth - spawnOffset, Random.Range(camPos.y - camHeight, camPos.y + camHeight)); break;
        }

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}