using UnityEngine;
using System.Collections;

public class GameFeelManager : MonoBehaviour
{
    [Header("Hit Stop (Zaman Donmasý)")]
    public float hitStopDuration = 0.05f;

    [Header("Kamera Sarsýntýsý (Screen Shake)")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.15f;

    private DynamicCamera dynamicCam;

    void Awake()
    {
        dynamicCam = GetComponent<DynamicCamera>();
    }

    void OnEnable()
    {
        // Radyoyu dinlemeye baþla (Artýk kimlik ve hasar bilgisi geliyor)
        GameEvents.OnPlayerDamaged += ApplyGameFeel;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerDamaged -= ApplyGameFeel;
    }

    // Radyodan gelen playerIndex'i alýyoruz ama þimdilik ekranda tek kamera olduðu için
    // kim hasar yerse yesin ekraný sallýyoruz. (Ýleride split-screen yaparsan burasý iþine yarar)
    private void ApplyGameFeel(int playerIndex, int damage)
    {
        StartCoroutine(HitStopRoutine());
        StartCoroutine(ScreenShakeRoutine());
    }

    private IEnumerator HitStopRoutine()
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }

    private IEnumerator ScreenShakeRoutine()
    {
        if (dynamicCam != null) dynamicCam.enabled = false;

        float elapsed = 0f;
        Vector3 originalPos = transform.position;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        transform.position = originalPos;
        if (dynamicCam != null) dynamicCam.enabled = true;
    }
}