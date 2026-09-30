using UnityEngine;

public class PlayerExpCollector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ExpGem"))
        {
            ExpGem gem = col.GetComponent<ExpGem>();
            if (gem != null)
            {
                // Toplanan EXP'yi kendi cebine değil, takımın ortak havuzuna yolla!
                if (TeamExperienceManager.Instance != null)
                {
                    TeamExperienceManager.Instance.AddExp(gem.expValue);
                }
                
                Destroy(col.gameObject); // Taşı sahneden sil
            }
        }
    }
}