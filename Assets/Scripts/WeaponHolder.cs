using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public class IntIntEvent : UnityEvent<int, int> { }

[System.Serializable]
public class WeaponSOEvent : UnityEvent<WeaponSO> { }

public class WeaponHolder : MonoBehaviour
{
    public static event System.Action<Vector3> OnWeaponFired;

    [SerializeField] WeaponSO pistolWeapon;
    [SerializeField] WeaponSO rifleWeapon;
    [SerializeField] IntIntEvent onAmmoChanged;
    [SerializeField] WeaponSOEvent onWeaponSwitched;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip reloadClip;

    [SerializeField] float minFirePitch = 0.95f;
    [SerializeField] float maxFirePitch = 1.05f;

    private WeaponSO currentWeapon;
    private GameObject currentWeaponInstance;
    private Transform bulletSpawnPoint;
    private float fireCooldownTimer;

    private Animator animator;
    private Transform handBone;

    private int pistolAmmo;
    private int rifleAmmo;
    private int handsLayerIndex;
    private bool isReloading;
    private bool reloadStateEntered;

    private int CurrentAmmo
    {
        get => currentWeapon == pistolWeapon ? pistolAmmo : rifleAmmo;
        set
        {
            if (currentWeapon == pistolWeapon) pistolAmmo = value;
            else rifleAmmo = value;
        }
    }

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        handsLayerIndex = animator.GetLayerIndex("Hands");
    }

    void Start()
    {
        handBone = animator.GetBoneTransform(HumanBodyBones.RightHand);
        pistolAmmo = pistolWeapon.clipSize;
        rifleAmmo = rifleWeapon.clipSize;
        EquipWeapon(pistolWeapon);
    }

    void Update()
    {
        HandleFireCooldownTimer();
        HandleReloading();
        HandleWeaponSwitch();
    }

    private void HandleWeaponSwitch()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SwitchWeapon(rifleWeapon);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SwitchWeapon(pistolWeapon);
        }
    }

    private void SwitchWeapon(WeaponSO weapon)
    {
        if (weapon == currentWeapon) return;
        EquipWeapon(weapon);
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
        if (isReloading || CurrentAmmo >= currentWeapon.clipSize) return;
        isReloading = true;
        reloadStateEntered = false;
        animator.SetTrigger("Reload");
        audioSource.PlayOneShot(reloadClip);
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
            CurrentAmmo = currentWeapon.clipSize;
            onAmmoChanged.Invoke(CurrentAmmo, currentWeapon.clipSize);
        }
    }

    void EquipWeapon(WeaponSO weapon)
    {
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }
        currentWeapon = weapon;
        isReloading = false;

        currentWeaponInstance = Instantiate(weapon.weaponPrefab, handBone);
        currentWeaponInstance.transform.localPosition = weapon.gripPosition;
        currentWeaponInstance.transform.localRotation = Quaternion.Euler(weapon.gripRotation);

        onAmmoChanged.Invoke(CurrentAmmo, currentWeapon.clipSize);
        Vector3 parentScale = handBone.lossyScale;
        currentWeaponInstance.transform.localScale = new Vector3 (
            1f / parentScale.x,
            1f / parentScale.y ,
            1f / parentScale.z) * 2f * weapon.weaponScale;

        bulletSpawnPoint = currentWeaponInstance.transform.Find("Muzzle");
        animator.SetInteger("WeaponType",(int)weapon.weaponType);
        onWeaponSwitched.Invoke(weapon);
    }

    public void FireWeapon()
    {
        if (fireCooldownTimer > 0f || isReloading || CurrentAmmo <= 0) return;
        Instantiate(currentWeapon.bulletPrefab,bulletSpawnPoint.position, transform.rotation);
        Instantiate(currentWeapon.muzzleFlashPrefab,
        bulletSpawnPoint.position,
        bulletSpawnPoint.rotation * Quaternion.Euler(0f, -90f, 0f),
        bulletSpawnPoint);
        animator.SetTrigger("Fire");
        audioSource.pitch = Random.Range(minFirePitch,maxFirePitch);
        audioSource.PlayOneShot(currentWeapon.fireSound);
        OnWeaponFired?.Invoke(transform.position);
        fireCooldownTimer = currentWeapon.fireRate;
        CurrentAmmo--;
        onAmmoChanged.Invoke(CurrentAmmo, currentWeapon.clipSize);
    }

    public void PlayHitReaction()
    {
        animator.SetTrigger("Hit");
    }
}
