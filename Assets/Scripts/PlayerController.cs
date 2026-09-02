using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float rotationSpeed = 720f;
    [SerializeField] FootstepAudio footstepAudio;
    [SerializeField] float stepDistance = 2f;

    private CharacterController characterController;
    private Camera mainCamera;
    private Animator animator;
    private WeaponHolder weaponHolder;
    private NavMeshAgent agent;

    private float distanceAccumulator;
    private Vector3 lastFootstepPosition;

    private Vector2 moveInput;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        mainCamera = Camera.main;
        weaponHolder = GetComponent<WeaponHolder>();

        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;

        lastFootstepPosition = transform.position;
    }


    void Update()
    {
        HandleAiming();
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 move = camForward * moveInput.y + camRight * moveInput.x;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        Vector3 localMove = transform.InverseTransformDirection(move);
        animator.SetFloat("MoveX", localMove.x);
        animator.SetFloat("MoveZ",localMove.z);

        HandleFootsteps(move);
    }

    private void HandleFootsteps(Vector3 move)
    {
        float distanceThisFrame = Vector3.Distance(transform.position, lastFootstepPosition);
        lastFootstepPosition = transform.position;

        if (move.sqrMagnitude <= 0.01f) return;

        distanceAccumulator += distanceThisFrame;
        if (distanceAccumulator >= stepDistance)
        {
            distanceAccumulator = 0f;
            footstepAudio.PlayFootstep();
        }
    }


    private void HandleAiming()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 remmappedPos = new Vector2(
            mouseScreenPos.x / Screen.width * mainCamera.pixelWidth,
            mouseScreenPos.y / Screen.height * mainCamera.pixelHeight);
        Ray ray = mainCamera.ScreenPointToRay(remmappedPos); 
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 aimPoint = ray.GetPoint(distance);
            Vector3 lookDir = aimPoint - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleShoot()
    {
        return;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnShoot(InputValue value)
    {
        if (value.isPressed)
        {
            weaponHolder.FireWeapon();
        }
    }

    public void OnReload(InputValue value)
    {
        if (value.isPressed)
        {
            weaponHolder.ReloadWeapon();
        }
    }
    
}
