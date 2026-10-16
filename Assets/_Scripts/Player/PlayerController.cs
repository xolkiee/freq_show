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

    [Header("Nişan (Aim) Ayarları")]
    private Vector2 aimInput;
    private bool isGamepad; // Oyuncu Gamepad mi yoksa Fare mi kullanıyor?

    [Header("Yetenek (Upgrade) Ayarları")]
    public GameObject fireTrailPrefab; // Yere bırakılacak ateşin prefabı
    private PlayerUpgradeManager upgradeManager; // Karakterin hafızası

    [Header("Bileşenler")]
    private Rigidbody2D rb;
    private Camera mainCamera;
    private PlayerInput playerInput; // Hangi cihazı kullandığımızı anlamak için

    // --- Orijinal katman hafızası ---
    private int originalLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        upgradeManager = GetComponent<PlayerUpgradeManager>(); // Hafızayı bul

        // Oyun başladığında oyuncunun varsayılan fizik katmanını kaydet
        originalLayer = gameObject.layer;
    }

    void Update()
    {
        // O anki oyuncunun Gamepad kullanıp kullanmadığını algıla
        isGamepad = playerInput.currentControlScheme == "Gamepad";

        // Dash bekleme süresini (Cooldown) say
        if (dashTimer > 0) dashTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        // Dash atarken fizik motoruna müdahale etmiyoruz
        if (isDashing) return;

        // 1. HAREKET UYGULAMA (Input sistemi WASD ve Analog çubuğu otomatik normalize eder)
        rb.velocity = moveInput * moveSpeed;

        // 2. NİŞAN ALMA UYGULAMA (Twin-Stick Mantığı)
        if (isGamepad)
        {
            // GAMEPAD: Sağ analog çubuğun itildiği yöne dön
            if (aimInput.sqrMagnitude > 0.01f) // Çubuk bırakıldığında karakterin açısının sıfırlanmasını önler
            {
                float aimAngle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg - 90f;
                rb.rotation = aimAngle;
            }
        }
        else
        {
            // KLAVYE/FARE: Ekranda farenin bulunduğu dünya koordinatına doğru dön
            Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(aimInput);
            Vector2 aimDirection = mouseWorldPosition - rb.position;
            float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = aimAngle;
        }
    }

    // =================================================================
    // YENİ INPUT SİSTEMİ MESAJLARI (Player Input "Send Messages" modunda bunları tetikler)
    // =================================================================

    // Sol Analog (Gamepad) veya WASD (Klavye)
    void OnMove(InputValue value)
    {
        // Dash atarken bile parmağını çektiğini (veya yön değiştirdiğini) arka planda hafızaya almalıyız!
        moveInput = value.Get<Vector2>();
    }

    // Sağ Analog (Gamepad) veya Fare Pozisyonu
    void OnAim(InputValue value)
    {
        aimInput = value.Get<Vector2>();
    }

    // Space (Klavye) veya B/Daire (Gamepad)
    void OnDash(InputValue value)
    {
        if (value.isPressed && dashTimer <= 0 && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    // Sol Tık (Fare) veya Right Bumper [RB] (Gamepad)

    // =================================================================

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

        //  DASH SIRASINDA YERE ATEŞ BIRAKMA DÖNGÜSÜ
        // Hafızadan FireDash yeteneğinin değerini çekiyoruz. (Yoksa 0 döner).
        float fireDashLifeTime = upgradeManager.GetUpgradeValue(UpgradeID.FireDash);
        
        float timePassed = 0f;
        float fireSpawnInterval = 0.1f; // Ne sıklıkla ateş bırakılacak 
        float nextFireSpawnTime = 0f;

        // Dash süresi boyunca her frame'de çalışacak bir While döngüsü kuruyoruz
        while (timePassed < dashDuration)
        {
            // Eğer karakterde FireDash yeteneği varsa ve ateş bırakma süresi geldiyse
            if (fireDashLifeTime > 0 && timePassed >= nextFireSpawnTime)
            {
                // Yere bir ateş prefabı kopyala
                GameObject fire = Instantiate(fireTrailPrefab, transform.position, Quaternion.identity);
                // Ateşin hasarını ayarla ve ne kadar süre kalacağını belirle
                fire.GetComponent<FireTrail>().Setup(2, fireDashLifeTime);
                
                nextFireSpawnTime += fireSpawnInterval;
            }

            timePassed += Time.deltaTime;
            yield return null; // Bir sonraki frame'i bekle
        }
        // ========================================================

        // --- I-FRAME BİTİŞİ: Karakteri orijinal (vurulabilir) katmanına geri döndür ---
        gameObject.layer = originalLayer;

        isDashing = false; // Kilitleri aç
    }
}