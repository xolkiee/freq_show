using UnityEngine;

public class AudioAnalyzer : MonoBehaviour
{
    private AudioSource audioSource;
    public float[] samples = new float[512];

    [Header("1. Sub-Bass Ayarlarý (20Hz - 60Hz)")]
    public float subMin = 20f;
    public float subMax = 60f;
    public float subThreshold = 15f;

    [Header("2. Kick Ayarlarý (110Hz - 180Hz)")]
    public float kickMin = 110f;
    public float kickMax = 180f;
    public float kickThreshold = 35f;
    public float kickCooldown = 0.15f;
    private float nextKickTime;

    [Header("3. Hi-Hat Ayarlarý (7500Hz - 10500Hz)")]
    // Aðý çok daha dar ve spesifik bir alana çektik
    public float hihatMin = 7500f;
    public float hihatMax = 10500f;
    public float hihatThreshold = 8f; // Aralýðý daralttýðýmýz için eþiði düþürdük
    public float hihatCooldown = 0.05f; // 16'lýk ve 32'lik seri vuruþlar için hala çok kýsa
    private float nextHiHatTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);

        // --- SUB-BASS ---
        float subPeak = GetFrequencyBandPeak(subMin, subMax) * 100f;
        if (subPeak > subThreshold)
        {
            GameEvents.TriggerSubIntensity(subPeak);
        }
        else
        {
            GameEvents.TriggerSubIntensity(0f);
        }

        // --- KICK ---
        float kickPeak = GetFrequencyBandPeak(kickMin, kickMax) * 100f;
        if (kickPeak > kickThreshold && Time.time > nextKickTime)
        {
            GameEvents.TriggerKickHit();
            nextKickTime = Time.time + kickCooldown;
        }

        // --- HI-HAT ---
        float hihatPeak = GetFrequencyBandPeak(hihatMin, hihatMax) * 100f;
        if (hihatPeak > hihatThreshold && Time.time > nextHiHatTime)
        {
            GameEvents.TriggerHiHatHit();
            nextHiHatTime = Time.time + hihatCooldown;
        }
    }

    private float GetFrequencyBandPeak(float minHz, float maxHz)
    {
        float nyquist = AudioSettings.outputSampleRate / 2f;

        int minIndex = Mathf.FloorToInt((minHz / nyquist) * samples.Length);
        int maxIndex = Mathf.FloorToInt((maxHz / nyquist) * samples.Length);

        minIndex = Mathf.Clamp(minIndex, 0, samples.Length - 1);
        maxIndex = Mathf.Clamp(maxIndex, 0, samples.Length - 1);

        float peakValue = 0f;
        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (samples[i] > peakValue) peakValue = samples[i];
        }

        return peakValue;
    }
}