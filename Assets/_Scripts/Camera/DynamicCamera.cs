using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicCamera : MonoBehaviour
{
    // Singleton yapýsý (Teleport veya dýþarýdan kolayca çaðýrmak için)
    public static DynamicCamera Instance;

    [Header("Takip Edilecek Hedefler")]
    public List<Transform> targets = new List<Transform>();

    [Header("Kamera Hareket Ayarlarý")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothTime = 0.3f;
    private Vector3 velocity;

    [Header("Zoom (Orthographic Size) Ayarlarý")]
    public float minZoom = 7f;
    public float maxZoom = 9f;
    public float zoomLimiter = 12f;

    [Header("Harita Sýnýrlarý (BoxCollider2D)")]
    public bool enableBounds = true;
    [Tooltip("Kameranýn þu an içinde bulunduðu odanýn sýnýr collider'ý")]
    public BoxCollider2D currentBounds;

    [Header("Kamera Sýnýrlarý")]
    public float screenPadding = 0.5f;

    private Camera cam;

    void Awake()
    {
        // Singleton atamasý
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        cam = GetComponent<Camera>();

        FindAllPlayers();

        if (targets.Count > 0)
        {
            SnapToMidpoint(); // Oyun baþlar baþlamaz kamerayý anýnda hedefe oturt
        }
    }

    void OnEnable()
    {
        InvokeRepeating(nameof(FindAllPlayers), 0f, 0.5f);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(FindAllPlayers));
    }

    void FindAllPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        targets.Clear();

        foreach (GameObject p in players)
        {
            if (p.activeInHierarchy)
            {
                targets.Add(p.transform);
            }
        }
    }

    // --- YENÝ: SINIRLARI GÜNCELLEME METODU ---
    public void SetNewBounds(BoxCollider2D newBounds)
    {
        currentBounds = newBounds;
    }

    // --- YENÝ: KAMERAYI ANINDA IÞINLAMA METODU ---
    // Portal'dan geçildiðinde kameranýn kayarak gitmemesi için çaðrýlacak
    public void SnapToMidpoint()
    {
        if (targets.Count == 0) return;

        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        if (enableBounds && currentBounds != null)
        {
            newPosition = ClampPositionToBounds(newPosition);
        }

        transform.position = newPosition;
    }

    void LateUpdate()
    {
        if (targets.Count == 0) return;

        Move();
        Zoom();
        ClampTargetsToCameraBounds();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        if (enableBounds && currentBounds != null)
        {
            newPosition = ClampPositionToBounds(newPosition);
        }

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    // Pozisyonu collider sýnýrlarýna göre kýsýtlayan yardýmcý fonksiyon
    Vector3 ClampPositionToBounds(Vector3 targetPos)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float minX = currentBounds.bounds.min.x + camWidth;
        float maxX = currentBounds.bounds.max.x - camWidth;
        float minY = currentBounds.bounds.min.y + camHeight;
        float maxY = currentBounds.bounds.max.y - camHeight;

        // Oda çok küçükse (kamera odadan büyükse) kamerayý odanýn merkezine kilitle
        if (maxX < minX)
        {
            minX = currentBounds.bounds.center.x;
            maxX = currentBounds.bounds.center.x;
        }
        if (maxY < minY)
        {
            minY = currentBounds.bounds.center.y;
            maxY = currentBounds.bounds.center.y;
        }

        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

        return new Vector3(clampedX, clampedY, offset.z);
    }

    void Zoom()
    {
        float greatestDistance = GetGreatestDistance();
        float targetZoom = (targets.Count == 1) ? minZoom : Mathf.Lerp(minZoom, maxZoom, greatestDistance / zoomLimiter);
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 3f);
    }

    void ClampTargetsToCameraBounds()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        Vector3 camPos = transform.position;

        float minX = camPos.x - camWidth + screenPadding;
        float maxX = camPos.x + camWidth - screenPadding;
        float minY = camPos.y - camHeight + screenPadding;
        float maxY = camPos.y + camHeight - screenPadding;

        foreach (Transform target in targets)
        {
            if (target == null || !target.gameObject.activeInHierarchy) continue;

            Vector3 clampedPos = target.position;
            clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
            clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);

            target.position = clampedPos;
        }
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1) return targets[0].position;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null && targets[i].gameObject.activeInHierarchy)
                bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }

    float GetGreatestDistance()
    {
        if (targets.Count == 1) return 0f;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null && targets[i].gameObject.activeInHierarchy)
                bounds.Encapsulate(targets[i].position);
        }
        return Mathf.Max(bounds.size.x, bounds.size.y);
    }
}