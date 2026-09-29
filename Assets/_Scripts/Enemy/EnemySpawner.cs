using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f; // Başlangıçta 2 saniyede bir düşman gelsin
    private float nextSpawnTime;

    private Camera mainCam;
    public float spawnOffset = 2f; // Kameranın sınırından ne kadar dışarıda doğsunlar?

    void Start()
    {
        mainCam = Camera.main;
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        // Zamanlayıcı doldu mu kontrol et
        if (Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + spawnInterval;
            SpawnEnemyOutsideCamera();
        }
    }

    void SpawnEnemyOutsideCamera()
    {
        // Kameranın o anki fiziksel genişliğini ve yüksekliğini hesapla
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