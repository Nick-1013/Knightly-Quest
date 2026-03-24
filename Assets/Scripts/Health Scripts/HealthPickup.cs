using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 25f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        if (health != null && health.isPlayer)
        {
            health.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}