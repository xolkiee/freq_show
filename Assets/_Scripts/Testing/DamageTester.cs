using UnityEngine;

public class DamageTester : MonoBehaviour
{
    void Start()
    {
        // Oyun baþladýktan 2 saniye sonra baþla, ve her 2 saniyede bir Vur metodunu çalýþtýr.
        InvokeRepeating("Vur", 2f, 2f);
    }

    void Vur()
    {
        // ARTIK TriggerPlayerDamaged DEÐÝL, Giriþim metodunu çaðýrýyoruz
        GameEvents.TriggerDamageAttempt(20);
    }
}