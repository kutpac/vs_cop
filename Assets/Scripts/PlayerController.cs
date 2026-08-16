using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float rotationSpeed = 720f;

    private CharacterController characterController;
    private Camera mainCamera;
    private Animator animator;
    private WeaponHolder weaponHolder;
    private NavMeshAgent agent;
    
    
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
    }


    void Update()
    {
        HandleAiming();
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        characterController.Move(move * moveSpeed * Time.deltaTime);

        Vector3 localMove = transform.InverseTransformDirection(move);
        animator.SetFloat("MoveX", localMove.x);
        animator.SetFloat("MoveZ",localMove.z);
    }


    private void HandleAiming()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos); 
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
    
}
