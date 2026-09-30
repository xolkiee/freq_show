using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stoppingDistance = 10f;

    public Transform playerTarget;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Düşman doğduğu andan itibaren her yarım saniyede bir radarını tarar
        InvokeRepeating(nameof(FindNearestPlayer), 0f, 0.5f);
    }

    // --- YENİ AGGRO (RADAR) SİSTEMİ ---
    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float minDistance = Mathf.Infinity;
        Transform nearest = null;

        // Bütün oyuncuları tek tek ölç ve en yakınını bul
        foreach (GameObject p in players)
        {
            if (!p.activeInHierarchy) continue; // Ölü (gizlenmiş) oyuncuyu görmezden gel

            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = p.transform;
            }
        }

        // Hedefi en yakındaki oyuncu olarak onayla
        playerTarget = nearest;
    }

    void FixedUpdate()
    {
        // Eğer sahnede yaşayan oyuncu kalmadıysa fren yap ve bekle
        if (playerTarget == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Oyuncuyla düşman arasındaki tam mesafeyi ölç
        float distance = Vector2.Distance(transform.position, playerTarget.position);
        Vector2 direction = (playerTarget.position - transform.position).normalized;

        // UFAK DOKUNUŞ: Düşmanın yüzünü her zaman oyuncuya döndür (Dururken bile nişan alabilsin)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;

        // Eğer aradaki mesafe belirlediğimiz sınırdan BÜYÜKSE kovalamaya devam et
        if (distance > stoppingDistance)
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            // Yeterince yakınsa dur ama rotasyon (yukarıdaki kod sayesinde) takibe devam etsin
            rb.velocity = Vector2.zero;
        }
    }
}