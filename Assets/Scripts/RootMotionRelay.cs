using UnityEngine;

public class RootMotionRelay : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;

    void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponentInParent<CharacterController>();
    }

    void OnAnimatorMove()
    {
        characterController.Move(animator.deltaPosition);
    }
}
