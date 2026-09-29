using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input Sistemi kütüphanesi

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 6f;
    private Vector2 moveInput;

    [Header("Dodge (Dash) Ayarları")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    [HideInInspector] public bool isDashing;
    private float dashTimer;

    [Header("Bileşenler")]
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 mousePosition;

    // --- YENİ EKLENEN: Orijinal katman hafızası ---
    private int originalLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // Oyun başladığında oyuncunun varsayılan fizik katmanını kaydet
        originalLayer = gameObject.layer;
    }

    void Update()
    {
        // Eğer dash atıyorsak başka bir girdi almasını engelliyoruz
        if (isDashing) return;

        // 1. HAREKET GİRDİSİ (WASD) - Geçici olarak direkt klavyeden okuyoruz, co-op yaparken PlayerInput'a bağlayacağız
        moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        }

        // 2. NİŞAN ALMA GİRDİSİ (Mouse Konumu)
        if (Mouse.current != null)
        {
            mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        // 3. DODGE GİRDİSİ (Space Tuşu)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && dashTimer <= 0)
        {
            StartCoroutine(DashRoutine());
        }

        // Dash bekleme süresini (Cooldown) say
        if (dashTimer > 0) dashTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        // Dash atarken fizik motoruna müdahale etmiyoruz
        if (isDashing) return;

        // Karakteri yürüt (Vektörü normalize ediyoruz ki çapraz giderken 2 kat hızlanmasın)
        rb.velocity = moveInput.normalized * moveSpeed;

        // Karakteri Mouse imlecine doğru döndür (Twin-Stick mantığı)
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f; // Yüzünü farenin olduğu yere dönmesi için -90 derece ofset
        rb.rotation = aimAngle;
    }

    // Dodge (Dash) Mekaniğini yöneten asenkron fonksiyon
    private IEnumerator DashRoutine()
    {
        isDashing = true; // Hareketi ve yeni inputları kilitler
        dashTimer = dashCooldown; // Cooldown'ı başlatır

        // --- I-FRAME BAŞLANGICI: Karakteri mermilerin içinden geçeceği hayalet katmana al ---
        gameObject.layer = LayerMask.NameToLayer("PlayerDodge");

        // Karakteri mevcut yönünde anlık olarak çok yüksek bir hıza ulaştırır
        if (moveInput != Vector2.zero)
            rb.velocity = moveInput.normalized * dashSpeed;
        else
            rb.velocity = transform.up * dashSpeed; // Durduğu yerde basarsa baktığı yöne atılır

        // Dash süresi kadar bekle (0.15 saniye)
        yield return new WaitForSeconds(dashDuration);

        // --- I-FRAME BİTİŞİ: Karakteri orijinal (vurulabilir) katmanına geri döndür ---
        gameObject.layer = originalLayer;

        isDashing = false; // Kilitleri aç
    }
}