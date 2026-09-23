using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input Sistemi kütüphanesi

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 6f;
    private Vector2 moveInput;

    [Header("Dodge (Dash) Ayarlarý")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    private bool isDashing;
    private float dashTimer;

    [Header("Bileþenler")]
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 mousePosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Eðer dash atýyorsak baþka bir girdi almasýný engelliyoruz
        if (isDashing) return;

        // 1. HAREKET GÝRDÝSÝ (WASD) - Geçici olarak direkt klavyeden okuyoruz, co-op yaparken PlayerInput'a baðlayacaðýz
        moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        }

        // 2. NÝÞAN ALMA GÝRDÝSÝ (Mouse Konumu)
        if (Mouse.current != null)
        {
            mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        // 3. DODGE GÝRDÝSÝ (Space Tuþu)
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

        // Karakteri yürüt (Vektörü normalize ediyoruz ki çapraz giderken 2 kat hýzlanmasýn)
        rb.velocity = moveInput.normalized * moveSpeed;

        // Karakteri Mouse imlecine doðru döndür (Twin-Stick mantýðý)
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f; // Yüzünü farenin olduðu yere dönmesi için -90 derece ofset
        rb.rotation = aimAngle;
    }

    // Dodge (Dash) Mekaniðini yöneten asenkron fonksiyon
    private IEnumerator DashRoutine()
    {
        isDashing = true; // Hareketi ve yeni inputlarý kilitler
        dashTimer = dashCooldown; // Cooldown'ý baþlatýr

        // Ýleride buraya i-frames (yenilmezlik) kodunu ve partikül efektini ekleyeceðiz

        // Karakteri mevcut yönünde anlýk olarak çok yüksek bir hýza ulaþtýrýr
        if (moveInput != Vector2.zero)
            rb.velocity = moveInput.normalized * dashSpeed;
        else
            rb.velocity = transform.up * dashSpeed; // Durduðu yerde basarsa baktýðý yöne atýlýr

        // Dash süresi kadar bekle (0.15 saniye)
        yield return new WaitForSeconds(dashDuration);

        isDashing = false; // Kilitleri aç
    }
}