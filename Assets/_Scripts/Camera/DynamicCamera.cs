using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicCamera : MonoBehaviour
{
    [Header("Takip Edilecek Hedefler")]
    public List<Transform> targets;

    [Header("Kamera Hareket Ayarlarý")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothTime = 0.3f;
    private Vector3 velocity;

    [Header("Zoom (Orthographic Size) Ayarlarý")]
    public float minZoom = 7f;  // Tek oyunculuda veya yan yanayken ne kadar yakýnlaþsýn
    public float maxZoom = 9f;  // CO-OP ÝÇÝN KESÝN SINIR: Ne kadar uzaklaþýrlarsa uzaklaþsýnlar bu boyutu geçemez
    public float zoomLimiter = 12f;

    [Header("Harita Sýnýrlarý (Dýþarýyý Göstermemek Ýçin)")]
    // 2048x2048 (20x20 birim) haritanýza göre bu deðerleri Inspector'dan ince ayar yapabilirsiniz
    public bool enableBounds = true;
    public Vector2 minBounds = new Vector2(-32f, -32f);
    public Vector2 maxBounds = new Vector2(32f, 32f);

    [Header("Kamera Sýnýrlarý")]
    public float screenPadding = 0.5f; // Karakterin kafasý veya gövdesi ekrandan taþmasýn diye ufak bir pay

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        // Baþlangýçta kameranýn aniden sýçramasýný engellemek için direkt konuma git
        if (targets.Count > 0)
        {
            Vector3 center = GetCenterPoint();
            transform.position = new Vector3(center.x, center.y, offset.z);
        }
    }

    void LateUpdate()
    {
        if (targets.Count == 0) return;

        Move();
        Zoom();

        // Kamera hareketini ve zoom'unu bitirdikten SONRA oyuncularý içeri hapseder
        ClampTargetsToCameraBounds();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        // Harita dýþýný göstermeyi engellemek için Kamera Pozisyonunu Sýnýrla (Clamp)
        if (enableBounds)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = cam.orthographicSize * cam.aspect;

            // Kameranýn gidebileceði maksimum noktalarý hesapla
            float clampedX = Mathf.Clamp(newPosition.x, minBounds.x + camWidth, maxBounds.x - camWidth);
            float clampedY = Mathf.Clamp(newPosition.y, minBounds.y + camHeight, maxBounds.y - camHeight);

            newPosition = new Vector3(clampedX, clampedY, offset.z);
        }

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float greatestDistance = GetGreatestDistance();

        // Mesafe ile zoom u oranla
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, greatestDistance / zoomLimiter);

        // KESÝN SINIRLAMA: targetZoom deðeri ne olursa olsun minZoom ile maxZoom arasýnda hapsolur
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // Pürüzsüz geçiþ (Unity'de kamera boyutu Orthographic Size ile deðiþtirilir)
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 3f);
    }

    void ClampTargetsToCameraBounds()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        Vector3 camPos = transform.position;

        // Ekranýn köþelerini hesapla ve padding (pay) ekleyerek daralt
        float minX = camPos.x - camWidth + screenPadding;
        float maxX = camPos.x + camWidth - screenPadding;
        float minY = camPos.y - camHeight + screenPadding;
        float maxY = camPos.y + camHeight - screenPadding;

        // Listedeki tüm oyuncularý kontrol et ve ekran dýþýna çýkanlarý durdur
        foreach (Transform target in targets)
        {
            if (target == null) continue;

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
            bounds.Encapsulate(targets[i].position);
        }
        return Mathf.Max(bounds.size.x, bounds.size.y);
    }
}