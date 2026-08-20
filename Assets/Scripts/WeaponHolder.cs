using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public static event System.Action<Vector3> OnWeaponFired;

    [SerializeField] WeaponSO currentWeapon;

    private GameObject currentWeaponInstance;
    private Transform bulletSpawnPoint;
    private float fireCooldownTimer;

    private Animator animator;
    private Transform handBone;


    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        handBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
        EquipWeapon(currentWeapon);
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

    void EquipWeapon(WeaponSO weapon)
    {
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }
        currentWeaponInstance = Instantiate(weapon.weaponPrefab, handBone);
        currentWeaponInstance.transform.localPosition = weapon.gripPosition;
        currentWeaponInstance.transform.localRotation = Quaternion.Euler(weapon.gripRotation);

        Vector3 parentScale = handBone.lossyScale;
        currentWeaponInstance.transform.localScale = new Vector3 (
            1f / parentScale.x,
            1f / parentScale.y , 
            1f / parentScale.z) * 2f * weapon.weaponScale;

        bulletSpawnPoint = currentWeaponInstance.transform.Find("Muzzle");
        animator.SetInteger("WeaponType",(int)weapon.weaponType);
    }

    public void FireWeapon()
    {
        if (fireCooldownTimer > 0f) return;
        Instantiate(currentWeapon.bulletPrefab,bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        animator.SetTrigger("Fire");
        OnWeaponFired?.Invoke(transform.position);
        fireCooldownTimer = currentWeapon.fireRate;
    }
}
