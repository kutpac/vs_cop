using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] UnityEvent onDeath;
    [SerializeField] UnityEvent onZombieDamaged;

    private float currentHealth;
    private bool isDead;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount,0);
        onZombieDamaged.Invoke();

        if (currentHealth <= 0f)
        {
            isDead = true;
            onDeath.Invoke();
        }
    }

    public void Revive(float newMaxHealth)
    {
        isDead = false;
        currentHealth = newMaxHealth;
    }
}
