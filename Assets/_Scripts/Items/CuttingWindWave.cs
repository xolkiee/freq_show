using UnityEngine;

public class CuttingWindWave : MonoBehaviour
{
    [HideInInspector] public int damage = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Çarptığımız obje düşman ise hasar ver ama rüzgarı silme (delip geçsin)
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}