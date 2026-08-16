using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float weaponScale = 0.68f;
    [SerializeField] float fireRate = 0.4f;
    
    private float fireCooldownTimer;
    private Animator animator;
    private Transform handBone;
    private Transform bulletSpawnPoint;
    private GameObject currentWeapon;

    private Vector3 pistolPosition = new Vector3(-0.05077881f, 0.1154558f, 0.02363466f);
    private Vector3 pistolRotation = new Vector3(264.41f,25.867f,77.643f);
    

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    
    void Start()
    {
        handBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
        EquipWeapon(weaponPrefab);
    }

    void Update()
    {
        HandleFireCooldownTimer();
    }

    private void HandleFireCooldownTimer()
    {
        if (fireCooldownTimer > 0f)
        {
            fireCooldownTimer -= Time.deltaTime;
        }
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
            1f / parentScale.z) * 2f * weaponScale;

        bulletSpawnPoint = currentWeapon.transform.Find("Muzzle");
    }
    
    public void FireWeapon()
    {
        
        if (fireCooldownTimer > 0f) return;
        Instantiate(bulletPrefab,bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        fireCooldownTimer = fireRate;
    }
}
