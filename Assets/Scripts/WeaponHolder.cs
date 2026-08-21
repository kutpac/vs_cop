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

    private int currentAmmo;
    private int handsLayerIndex;
    private bool isReloading;
    private bool reloadStateEntered;


    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        handsLayerIndex = animator.GetLayerIndex("Hands");
    }

    void Start()
    {
        handBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
        EquipWeapon(currentWeapon);
    }

    void Update()
    {
        HandleFireCooldownTimer();
        HandleReloading();
    }

    private void HandleFireCooldownTimer()
    {
        if (fireCooldownTimer > 0f)
        {
            fireCooldownTimer -= Time.deltaTime;
        }
    }

    public void ReloadWeapon()
    {
        if (isReloading || currentAmmo >= currentWeapon.clipSize) return;
        isReloading = true;
        reloadStateEntered = false;
        animator.SetTrigger("Reload");
    }

    private void HandleReloading()
    {
        if (!isReloading) return;
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(handsLayerIndex);

        if (!reloadStateEntered)
        {
            if(!state.IsName("Empty State"))
            {
                reloadStateEntered = true;
            }
            return;
        }

        if (state.IsName("Empty State"))
        {
            isReloading = false;
            currentAmmo = currentWeapon.clipSize;
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

        currentAmmo = weapon.clipSize;

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
        if (fireCooldownTimer > 0f || isReloading || currentAmmo <= 0) return;
        Instantiate(currentWeapon.bulletPrefab,bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        animator.SetTrigger("Fire");
        OnWeaponFired?.Invoke(transform.position);
        fireCooldownTimer = currentWeapon.fireRate;
        currentAmmo--;
    }
}
