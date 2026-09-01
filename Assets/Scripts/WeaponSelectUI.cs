using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectUI : MonoBehaviour
{
    [SerializeField] WeaponSO rifleWeapon;
    [SerializeField] WeaponSO pistolWeapon;
    [SerializeField] Image rifleImage;
    [SerializeField] Image pistolImage;

    public void UpdateSelection(WeaponSO activeWeapon)
    {
        rifleImage.enabled = activeWeapon == rifleWeapon;
        pistolImage.enabled = activeWeapon == pistolWeapon;
    }
}
