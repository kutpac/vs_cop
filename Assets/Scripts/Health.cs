using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] UnityEvent onDeath;
    [SerializeField] UnityEvent onZombieDamaged;

    protected float currentHealth;
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
        OnHealthChanged();

        if (currentHealth <= 0f)
        {
            isDead = true;
            onDeath.Invoke();
        }
    }
    protected virtual void OnHealthChanged() { }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged();
    }

    public void Revive(float newMaxHealth)
    {
        isDead = false;
        currentHealth = newMaxHealth;
    }
}
