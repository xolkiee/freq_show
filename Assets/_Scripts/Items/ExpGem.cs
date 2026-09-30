using UnityEngine;

public class ExpGem : MonoBehaviour
{
    public int expValue = 10;
    
    [Header("Mıknatıs Ayarları")]
    public float magnetRadius = 3f;  // Taşın oyuncuyu fark etme mesafesi
    public float moveSpeed = 8f;     // Oyuncuya doğru uçma hızı

    private Transform targetPlayer;

    void Update()
    {
        // Eğer takip edilecek bir oyuncu yoksa etrafı tara
        if (targetPlayer == null)
        {
            FindClosestPlayer();
        }
        else
        {
            // Hedef oyuncu bulunduysa ona doğru uç
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
        }
    }

    void FindClosestPlayer()
    {
        // Sahnede "Player" etiketine sahip tüm karakterleri bul (Co-op uyumlu)
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                
                // Eğer oyuncu mıknatıs menzilindeyse ve en yakındaki oyuncuysa hedefe kilitlen
                if (distance <= magnetRadius && distance < closestDistance)
                {
                    closestDistance = distance;
                    targetPlayer = player.transform;
                }
            }
        }
    }
}