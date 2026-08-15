using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    
    private Animator animator;
    private NavMeshAgent agent;

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
    }

    void Update()
    {
        HandleZombieMovement();
    }

    private void HandleZombieMovement()
    {
        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f)
        {
            agent.SetDestination(playerTransform.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);
            retargetTimer = retargetInterval;
        }
    }
}
