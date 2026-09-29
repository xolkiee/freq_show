using UnityEngine;

public class BeatVisualizer : MonoBehaviour
{
    // Küpün hangi frekans kanalýný dinleyeceðini seçeceðimiz menü
    public enum BandType { Kick, HiHat, SubBass }
    public BandType listenTo;

    private Vector3 startScale;

    [Header("Kick & Hi-Hat (Vuruþ) Ayarlarý")]
    public float beatScale = 1.5f;
    public float returnSpeed = 5f;

    [Header("Sub-Bass (Basýnç) Ayarlarý")]
    public float subMultiplier = 0.1f; // Gelen yüksek frekans deðerini dengelemek için
    private float targetSubScale = 1f;

    void Start()
    {
        startScale = transform.localScale;

        // Küpün görevine göre doðru radyo kanalýna abone ol
        if (listenTo == BandType.Kick)
            GameEvents.OnKickHit += Pulse;
        else if (listenTo == BandType.HiHat)
            GameEvents.OnHiHatHit += Pulse;
        else if (listenTo == BandType.SubBass)
            GameEvents.OnSubIntensity += UpdateSub;
    }

    void OnDestroy()
    {
        if (listenTo == BandType.Kick)
            GameEvents.OnKickHit -= Pulse;
        else if (listenTo == BandType.HiHat)
            GameEvents.OnHiHatHit -= Pulse;
        else if (listenTo == BandType.SubBass)
            GameEvents.OnSubIntensity -= UpdateSub;
    }

    void Update()
    {
        if (listenTo == BandType.Kick || listenTo == BandType.HiHat)
        {
            // Kick ve Hi-Hat: Aniden büyü, sonra yavaþça küçül
            transform.localScale = Vector3.Lerp(transform.localScale, startScale, Time.deltaTime * returnSpeed);
        }
        else if (listenTo == BandType.SubBass)
        {
            // Sub-Bass: Basýnç (Intensity) deðerine göre yumuþakça þiþ veya in
            Vector3 desiredScale = startScale * targetSubScale;
            transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 15f);
        }
    }

    private void Pulse()
    {
        // Anlýk darbe (Kick ve Hi-Hat için)
        transform.localScale = startScale * beatScale;
    }

    private void UpdateSub(float intensity)
    {
        // Sub-Bass þiddetine göre hedef boyutu güncelle
        targetSubScale = 1f + (intensity * subMultiplier);
    }
}