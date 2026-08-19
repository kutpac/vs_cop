using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] private int attackLayerIndex;

    [SerializeField] string[] idleFlavorStates;
    [SerializeField] float[] idleFlavorChances;

    [SerializeField] float fireSoundNoticeRadius =25f;
    [SerializeField] float noticeRadius = 8f;
    [SerializeField] float screamChance = 0.3f;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] int attackDamage = 10;
    [SerializeField] float hitStaggerDuration = 0.3f;
    [SerializeField] float hitSlowMultiplier = 0.2f;
    [SerializeField] float crawlChance = 0.3f;
    [SerializeField] float crawlHealth = 30f;
    [SerializeField] float crawlSpeedMultiplier = 0.3f; 
    [SerializeField] float crawlerDeathFreezeDelay = 1f;
    
    private Animator animator;
    private NavMeshAgent agent;

    private Health playerHealth;
    private Health health;

    private bool hasNoticedPlayer;
    private bool canMove;
    private float currentBaseSpeed;
    private float attackCooldownTimer;
    private bool hasStandupSpeedParam;
    private bool hasWillCrawlParam;
    private bool isDying;
    private bool diedFinal;
    private float baseAgentSpeed;
    private float hitStaggerTimer;
    private bool isCrawler;
    private bool frozenDeath;
    private float deathFreezeTimer;

    float retargetInterval = 0.25f;
    float retargetTimer;

    void OnEnable()
    {
        WeaponHolder.OnWeaponFired += HandleWeaponFired;
    }

    void OnDisable()
    {
        WeaponHolder.OnWeaponFired -= HandleWeaponFired;
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        attackLayerIndex = animator.GetLayerIndex("Attack");
        hasStandupSpeedParam = System.Array.Exists(animator.parameters, p => p.name == "StandupSpeed");
        hasWillCrawlParam = System.Array.Exists(animator.parameters, p => p.name == "WillCrawl");
        health = GetComponent<Health>();
        baseAgentSpeed = agent.speed;
        currentBaseSpeed = baseAgentSpeed;
    }

    void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        playerHealth = playerTransform.GetComponent<Health>();

        animator.Play(PickIdleFlavorState());
    }

    void Update()
    {
        if (isDying)
        {
            HandlePostDeathTransition();
            return;
        }
        HandleNoticing();
        HandleCanMove();
        HandleStaggerTimer();
        HandleAttacking();
        HandleZombieMovement();
    }

    private void HandleStaggerTimer()
    {
        if (hitStaggerTimer > 0f)
        {
            hitStaggerTimer -= Time.deltaTime;
            if (hitStaggerTimer <= 0f)
            {
                agent.speed = currentBaseSpeed;
            }
        }
    }

    private string PickIdleFlavorState()
    {
        float roll = Random.value;
        float cumulative = 0f;

        for (int i = 0; i < idleFlavorStates.Length; i++)
        {
            cumulative += idleFlavorChances[i];
            if (roll < cumulative)
            {
            return idleFlavorStates[i];
            }
        }
        return idleFlavorStates[idleFlavorChances.Length -1];
    }


    private void HandleNoticing()
    {
        if (hasNoticedPlayer) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= noticeRadius)
        {
            NoticePlayer();
        }
    }

    private void NoticePlayer()
    {
        hasNoticedPlayer = true;
        bool willScream = Random.value < screamChance;
        animator.SetBool("WillScream", willScream);
        animator.SetTrigger("Noticed");
        if (hasStandupSpeedParam)
        {
        animator.SetFloat("StandupSpeed", 1f);
        }
    }

    public void PlayHitReaction()
    {
        animator.SetLayerWeight(attackLayerIndex,1f);
        animator.SetTrigger(Random.value < 0.5f ? "Hit1" : "Hit2");
        hitStaggerTimer = hitStaggerDuration;
        agent.speed = currentBaseSpeed * hitSlowMultiplier;
        agent.velocity *= hitSlowMultiplier;
    }

    private void HandleWeaponFired(Vector3 firePosition)
    {
        if (hasNoticedPlayer) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= fireSoundNoticeRadius)
        {
            NoticePlayer();
        }
    }

    private void HandleAttacking()
    {
        if(!canMove) return;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= attackRange)
        {

            bool isAttacking = animator.GetCurrentAnimatorStateInfo(attackLayerIndex).IsName("Zombie Attack");
            if (isAttacking) return;

            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                animator.SetTrigger("Attack");
                animator.SetLayerWeight(attackLayerIndex,1f);
                playerHealth.TakeDamage(attackDamage);
                attackCooldownTimer = attackCooldown;
            }
        } 
    }

    private void HandleCanMove()
    {
        if (!hasNoticedPlayer || canMove) return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Moving"))
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
    
    public void HandleDeath()
    {
        isDying = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (isCrawler)
        {
            diedFinal = true;
            frozenDeath = true;
            animator.speed = 0f;
            agent.velocity = Vector3.zero;
            deathFreezeTimer = crawlerDeathFreezeDelay;
            return;
        }

        frozenDeath =false;
        bool willCrawl = hasWillCrawlParam && Random.value < crawlChance;
        diedFinal = !willCrawl;
        
        if (hasWillCrawlParam)
        {
            animator.SetBool("WillCrawl",willCrawl);
        }
        animator.SetTrigger("Die");

        if (willCrawl)
        {
            isCrawler = true;
            agent.speed = baseAgentSpeed * crawlSpeedMultiplier;
            currentBaseSpeed = agent.speed;
            health.Revive(crawlHealth);
        }
    }
    private void HandlePostDeathTransition()
    {
        if (frozenDeath)
        {
            deathFreezeTimer -= Time.deltaTime;
            if (deathFreezeTimer <= 0f)
            {
                gameObject.SetActive(false);
            }
            return;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (!diedFinal && state.IsName("Zombie Crawl"))
        {
            isDying = false;
            agent.isStopped=false;
            return;
        }
        if (diedFinal && state.IsName("Zombie Death") && state.normalizedTime >= 1f)
        {
            gameObject.SetActive(false);
        }
    }
}
