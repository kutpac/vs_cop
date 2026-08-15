using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float fireRate = 1f;

    private Animator animator;
    private Transform handBone;
    private GameObject currentWeapon;

    private Vector3 pistolPosition = new Vector3(0.00131f, 0.00271f, -0.00086f);
    private Vector3 pistolRotation = new Vector3(100.442f,105.307f,0f);

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        handBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
        EquipWeapon(weaponPrefab);
    }


    void EquipWeapon(GameObject prefab)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentWeapon = Instantiate(weaponPrefab, handBone);
        currentWeapon.transform.localPosition = pistolPosition;
        currentWeapon.transform.localRotation = Quaternion.Euler(pistolRotation);

        Vector3 parentScale = handBone.lossyScale;
        currentWeapon.transform.localScale = new Vector3 (
            1f / parentScale.x,
            1f / parentScale.y , 
            1f / parentScale.z);
    }
    
    public void FireWeapon()
    {
        //fireRate
        Instantiate(bulletPrefab,bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }
}
