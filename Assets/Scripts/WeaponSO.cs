using UnityEngine;

public enum WeaponType {Pistol, Rifle}

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]

public class WeaponSO : ScriptableObject
{
    public WeaponType weaponType;
    
    public GameObject weaponPrefab;
    public GameObject bulletPrefab;
    
    public float fireRate;
    public float damage;
    public int clipSize;

    public float weaponScale;
    public Vector3 gripPosition;
    public Vector3 gripRotation;
}
