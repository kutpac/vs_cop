using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatFloatEvent : UnityEvent<float, float> { }

public class PlayerHealth : Health
{
    [SerializeField] FloatFloatEvent onHealthChanged;

    protected override void OnHealthChanged()
    {
        onHealthChanged.Invoke(currentHealth,maxHealth);
    }
}
