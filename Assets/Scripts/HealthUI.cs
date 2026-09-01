using TMPro;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] TMP_Text healthText;
    
    public void UpdateHealth(float current, float max)
    {
        healthText.text = $"{Mathf.CeilToInt(current)}";
    }
}
