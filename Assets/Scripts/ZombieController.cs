using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float noticeRadius = 8f;
    [SerializeField] float biteChance = 0.5f;
    [SerializeField] float screamChance = 0.3f;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] int attackDamage = 10;
    
    private Animator animator;
    private NavMeshAgent agent;

    private Health playerHealth;
    private bool hasNoticedPlayer;
    private bool canMove;
    private float attackCooldownTimer;

    float retargetInterval = 0.25f;
    float retargetTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        playerHealth = playerTransform.GetComponent<Health>();

        if (Random.value < biteChance)
        {
            animator.Play("Biting Loop Forward");
        }
    }

    void Update()
    {
        HandleNoticing();
        HandleCanMove();
        HandleAttacking();
        HandleZombieMovement();
    }

    private void HandleNoticing()
    {
        if (hasNoticedPlayer) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= noticeRadius)
        {
            hasNoticedPlayer = true;
            bool willScream = Random.value < screamChance;
            animator.SetBool("WillScream", willScream);
            animator.SetTrigger("Noticed");
        }
    }

    private void HandleAttacking()
    {
        if(!canMove) return;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= attackRange)
        {

            bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack");
            if (isAttacking) return;

            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                animator.SetTrigger("Attack");
                playerHealth.TakeDamage(attackDamage);
                attackCooldownTimer = attackCooldown;
            }
        } 
    }

    private void HandleCanMove()
    {
        if (!hasNoticedPlayer || canMove) return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Walking"))
        {
            canMove = true;
        }
    }

    private void HandleZombieMovement()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);

        if (!canMove) return;

        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f)
        {
            agent.SetDestination(playerTransform.position);
            retargetTimer = retargetInterval;
        }
    }
}
