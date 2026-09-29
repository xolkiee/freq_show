using UnityEngine;
using System.Collections;

public class GameFeelManager : MonoBehaviour
{
    [Header("Hit Stop (Zaman Donmasý)")]
    public float hitStopDuration = 0.05f; // Gerçek dünyada ne kadar süre donuk kalacak?

    [Header("Kamera Sarsýntýsý (Screen Shake)")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.15f; // Ufak ve tatlý bir sarsýntý deðeri

    private DynamicCamera dynamicCam;

    void Awake()
    {
        // Ayný objede (Main Camera) bulunan kamera hareket kodumuzu buluyoruz
        dynamicCam = GetComponent<DynamicCamera>();
    }

    void OnEnable()
    {
        // Radyoyu dinlemeye baþla: Oyuncu hasar aldýðýnda 'ApplyGameFeel' metodunu çalýþtýr
        GameEvents.OnPlayerDamaged += ApplyGameFeel;
    }

    void OnDisable()
    {
        // Kapanýrken radyodan çýk
        GameEvents.OnPlayerDamaged -= ApplyGameFeel;
    }

    private void ApplyGameFeel(int damage)
    {
        // Ýki efekti ayný anda baþlatýyoruz
        StartCoroutine(HitStopRoutine());
        StartCoroutine(ScreenShakeRoutine());
    }

    private IEnumerator HitStopRoutine()
    {
        // Zamaný tamamen 0 yapmak yerine 0.05f gibi çok düþük bir deðere çekmek, 
        // oyunun "çökmüþ" gibi deðil, "Matrix" gibi yavaþlamýþ hissettirmesini saðlar.
        Time.timeScale = 0.05f;

        // DÝKKAT: Zamaný yavaþlattýðýmýz için normal 'WaitForSeconds' çalýþmaz (o da yavaþlar).
        // Bu yüzden 'Realtime' (Gerçek Dünya zamaný) kullanýyoruz.
        yield return new WaitForSecondsRealtime(hitStopDuration);

        // Zamaný normale döndür
        Time.timeScale = 1f;
    }

    private IEnumerator ScreenShakeRoutine()
    {
        // ÇOK KRÝTÝK: Kendi yazdýðýmýz DynamicCamera kodu her saniye kameranýn pozisyonunu güncelliyor.
        // Sarsýntý yaparken kamerayý onun elinden geçici olarak (0.1 saniyeliðine) almalýyýz, yoksa titreme olmaz.
        if (dynamicCam != null) dynamicCam.enabled = false;

        float elapsed = 0f;
        Vector3 originalPos = transform.position;

        while (elapsed < shakeDuration)
        {
            // Rastgele ufak X ve Y deðerleri üret
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            // Kamerayý bu rastgele konumlara taþý
            transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            // Zaman durmuþ (HitStop) olsa bile sarsýntýnýn devam etmesi için unscaledDeltaTime kullanýyoruz
            elapsed += Time.unscaledDeltaTime;

            yield return null; // Bir sonraki kareyi (frame) bekle
        }

        // Sarsýntý bitince kamerayý orijinal yerine koy ve DynamicCamera kodunu geri aç
        transform.position = originalPos;
        if (dynamicCam != null) dynamicCam.enabled = true;
    }
}