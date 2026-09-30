using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicCamera : MonoBehaviour
{
    [Header("Takip Edilecek Hedefler")]
    // Artýk bu listeyi Inspector'dan doldurmana gerek yok, kod kendi bulacak
    public List<Transform> targets = new List<Transform>();

    [Header("Kamera Hareket Ayarlarý")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothTime = 0.3f;
    private Vector3 velocity;

    [Header("Zoom (Orthographic Size) Ayarlarý")]
    public float minZoom = 7f;  // Tek oyunculuda veya yan yanayken ne kadar yakýnlaþsýn
    public float maxZoom = 9f;  // CO-OP ÝÇÝN KESÝN SINIR: En fazla ne kadar uzaklaþsýn
    public float zoomLimiter = 12f; // Karakterler ne kadar uzaklaþýnca maxZoom devreye girsin

    [Header("Harita Sýnýrlarý (Dýþarýyý Göstermemek Ýçin)")]
    public bool enableBounds = true;
    public Vector2 minBounds = new Vector2(-32f, -32f);
    public Vector2 maxBounds = new Vector2(32f, 32f);

    [Header("Kamera Sýnýrlarý")]
    public float screenPadding = 0.5f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Oyun baþladýðýnda sahnede hazýr oyuncu varsa listeye ekle
        FindAllPlayers();

        if (targets.Count > 0)
        {
            Vector3 center = GetCenterPoint();
            transform.position = new Vector3(center.x, center.y, offset.z);
        }
    }

    // --- YENÝ EKLENEN RADAR SÝSTEMÝ ---
    // Bu fonksiyonu her yarým saniyede bir veya oyuncu doðduðunda çaðýrabiliriz
    // Þu an için performans dostu olmasý adýna saniyede 2 kez (0.5s) taratýyoruz.
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
        // Sahnede "Player" etiketine (Tag) sahip tüm objeleri bul
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        targets.Clear(); // Listeyi temizle

        // Bulunan oyuncularý takibe al (eðer aktiflerse)
        foreach (GameObject p in players)
        {
            if (p.activeInHierarchy)
            {
                targets.Add(p.transform);
            }
        }
    }
    // -----------------------------------

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

        if (enableBounds)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = cam.orthographicSize * cam.aspect;

            float clampedX = Mathf.Clamp(newPosition.x, minBounds.x + camWidth, maxBounds.x - camWidth);
            float clampedY = Mathf.Clamp(newPosition.y, minBounds.y + camHeight, maxBounds.y - camHeight);

            newPosition = new Vector3(clampedX, clampedY, offset.z);
        }

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float greatestDistance = GetGreatestDistance();

        // Tek oyuncu varsa zoom'u minZoom'da tut, yoksa mesafeye göre hesapla
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