using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] TMP_Text ammoText;

    public void UpdateAmmo(int current, int max)
    {
        ammoText.text = $"{current}/{max}";
    }
}
