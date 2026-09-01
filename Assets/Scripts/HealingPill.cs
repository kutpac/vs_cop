using UnityEngine;

public class HealingPill : MonoBehaviour
{
    [SerializeField] float healAmount = 30f;

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        health.Heal(healAmount);
        Destroy(gameObject);
    }
}
