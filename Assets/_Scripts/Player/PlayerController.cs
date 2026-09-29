using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input Sistemi k�t�phanesi

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlar�")]
    public float moveSpeed = 6f;
    private Vector2 moveInput;

    [Header("Dodge (Dash) Ayarlar�")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    [HideInInspector] public bool isDashing;
    private float dashTimer;

    [Header("Bile�enler")]
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
        // E�er dash at�yorsak ba�ka bir girdi almas�n� engelliyoruz
        if (isDashing) return;

        // 1. HAREKET G�RD�S� (WASD) - Ge�ici olarak direkt klavyeden okuyoruz, co-op yaparken PlayerInput'a ba�layaca��z
        moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        }

        // 2. N��AN ALMA G�RD�S� (Mouse Konumu)
        if (Mouse.current != null)
        {
            mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        // 3. DODGE G�RD�S� (Space Tu�u)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && dashTimer <= 0)
        {
            StartCoroutine(DashRoutine());
        }

        // Dash bekleme s�resini (Cooldown) say
        if (dashTimer > 0) dashTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        // Dash atarken fizik motoruna m�dahale etmiyoruz
        if (isDashing) return;

        // Karakteri y�r�t (Vekt�r� normalize ediyoruz ki �apraz giderken 2 kat h�zlanmas�n)
        rb.velocity = moveInput.normalized * moveSpeed;

        // Karakteri Mouse imlecine do�ru d�nd�r (Twin-Stick mant���)
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f; // Y�z�n� farenin oldu�u yere d�nmesi i�in -90 derece ofset
        rb.rotation = aimAngle;
    }

    // Dodge (Dash) Mekani�ini y�neten asenkron fonksiyon
    private IEnumerator DashRoutine()
    {
        isDashing = true; // Hareketi ve yeni inputlar� kilitler
        dashTimer = dashCooldown; // Cooldown'� ba�lat�r

        // �leride buraya i-frames (yenilmezlik) kodunu ve partik�l efektini ekleyece�iz

        // Karakteri mevcut y�n�nde anl�k olarak �ok y�ksek bir h�za ula�t�r�r
        if (moveInput != Vector2.zero)
            rb.velocity = moveInput.normalized * dashSpeed;
        else
            rb.velocity = transform.up * dashSpeed; // Durdu�u yerde basarsa bakt��� y�ne at�l�r

        // Dash s�resi kadar bekle (0.15 saniye)
        yield return new WaitForSeconds(dashDuration);

        isDashing = false; // Kilitleri a�
    }
}