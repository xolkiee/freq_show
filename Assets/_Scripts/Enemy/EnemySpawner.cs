using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Temel Üretim Ayarları")]
    public GameObject enemyPrefab;
    public float baseSpawnInterval = 2f;   // Başlangıç hızı
    public float minSpawnInterval = 0.3f;  // Düşebileceği en düşük hız (Oyun çökmesin diye)
    public float spawnOffset = 2f; // Kameranın sınırından ne kadar dışarıda doğsunlar?

    [Header("Zorluk (Escalating Chaos) Ayarları")]
    public float difficultyIncreaseTimer = 10f; // Kaç saniyede bir oyun zorlaşacak?
    public float decreaseAmount = 0.15f;        // Her zorlaştığında süre ne kadar kısalacak?

    [Header("FFT (Müzik) Bağlantısı")]
    [Tooltip("Furkan'ın FFT kodu bas vurduğunda bu değeri düşürüp ritme göre düşman attıracak")]
    public float fftMultiplier = 1f; // 1 = Normal hız. Bas vurduğunda Furkan bunu 0.1 falan yapacak.

    private float nextSpawnTime;
    private float nextDifficultyIncrease;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        nextSpawnTime = Time.time + GetCurrentSpawnInterval();
        nextDifficultyIncrease = Time.time + difficultyIncreaseTimer;
    }

    void Update()
    {
        // 1. DİNAMİK ZORLUK: Süre doldukça base süreyi kısalt (Oyun kendi kendine zorlaşsın)
        if (Time.time >= nextDifficultyIncrease)
        {
            IncreaseDifficulty();
            nextDifficultyIncrease = Time.time + difficultyIncreaseTimer;
        }

        // 2. ÜRETİM: Zamanı geldikçe düşman fırlat
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemyOutsideCamera();
            nextSpawnTime = Time.time + GetCurrentSpawnInterval();
        }
    }

    void IncreaseDifficulty()
    {
        // Spawner hızını arttır ama minimum sınırdan daha aşağı inmesine izin verme
        if (baseSpawnInterval > minSpawnInterval)
        {
            baseSpawnInterval -= decreaseAmount;
            baseSpawnInterval = Mathf.Max(baseSpawnInterval, minSpawnInterval);
            Debug.Log("Oyun Zorlaştı! Yeni normal düşman gelme süresi: " + baseSpawnInterval);
        }
    }

    // FFT KANCASI BURASI: Üretim hızını belirleyen nihai matematik
    public float GetCurrentSpawnInterval()
    {
        // Temel üretim hızı ile müziğin ritmini çarpıyoruz. 
        // İleride Furkan FFT'den gelen şiddeti "fftMultiplier" değişkenine eşitleyecek.
        float finalInterval = baseSpawnInterval * fftMultiplier;
        
        // Ritme göre mermiler çok hızlansa bile 0.05 saniyenin altına düşmesin (Güvenlik)
        return Mathf.Max(finalInterval, 0.05f); 
    }

    void SpawnEnemyOutsideCamera()
    {
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector2 camPos = mainCam.transform.position;

        // Düşmanın çıkacağı rastgele bir kenar seç (0 = Üst, 1 = Alt, 2 = Sağ, 3 = Sol)
        int edge = Random.Range(0, 4);
        Vector2 spawnPosition = Vector2.zero;

        switch (edge)
        {
            case 0: // Üstten gelsin
                spawnPosition = new Vector2(Random.Range(camPos.x - camWidth, camPos.x + camWidth), camPos.y + camHeight + spawnOffset);
                break;
            case 1: // Alttan gelsin
                spawnPosition = new Vector2(Random.Range(camPos.x - camWidth, camPos.x + camWidth), camPos.y - camHeight - spawnOffset);
                break;
            case 2: // Sağdan gelsin
                spawnPosition = new Vector2(camPos.x + camWidth + spawnOffset, Random.Range(camPos.y - camHeight, camPos.y + camHeight));
                break;
            case 3: // Soldan gelsin
                spawnPosition = new Vector2(camPos.x - camWidth - spawnOffset, Random.Range(camPos.y - camHeight, camPos.y + camHeight));
                break;
        }

        // Hesaplanıp seçilen o görünmez noktada düşmanı yarat!
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}