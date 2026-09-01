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
    [SerializeField] int attackDamage = 6;
    [SerializeField] float hitStaggerDuration = 0.3f;
    [SerializeField] float hitSlowMultiplier = 0.2f;
    [SerializeField] float crawlChance = 0.3f;
    [SerializeField] float crawlHealth = 30f;
    [SerializeField] float crawlSpeedMultiplier = 0.3f; 
    [SerializeField] float crawlerDeathFreezeDelay = 1f;
    [SerializeField] float corpseLingerTime = 45f;
    
    private Animator animator;
    private NavMeshAgent agent;

    private Health playerHealth;
    private Health health;

    private Transform headBone;
    private Transform bodyBone;

    private CapsuleCollider capsuleCollider;

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
    private bool awaitingRemoval;
    private float corpseLingerTimer;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] idleSounds;
    [SerializeField] float minIdleInterval = 4f;
    [SerializeField] float maxIdleInterval = 10f;
    [SerializeField] AudioClip screamSound;
    [SerializeField] AudioClip[] hurtSounds;
    [SerializeField] AudioClip[] impactSounds;
    [SerializeField] AudioClip[] attackSounds;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] float voiceVolume = 0.3f;

    [SerializeField] GameObject healingPillPrefab;
    [SerializeField] float healingPillDropChance = 0.05f;

    private float idleSoundTimer;

    float retargetInterval = 0.25f;
    float retargetTimer;

    public bool IsDead => isDying;

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
        capsuleCollider = GetComponent<CapsuleCollider>();
        headBone = animator.GetBoneTransform(HumanBodyBones.Head);
        bodyBone = animator.GetBoneTransform(HumanBodyBones.Spine);
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
        HandleIdleSounds();
    }

    private void HandleIdleSounds()
    {
        if (hasNoticedPlayer) return;

        idleSoundTimer -= Time.deltaTime;
        if (idleSoundTimer <= 0f)
        {
            PlayRandomClip(idleSounds, voiceVolume);
            idleSoundTimer = Random.Range(minIdleInterval, maxIdleInterval);
        }
    }

    private void PlayRandomClip(AudioClip[] clips, float volume = 1f)
    {
        if (clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        audioSource.PlayOneShot(clip, volume);
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

        if (willScream)
        {
            audioSource.PlayOneShot(screamSound, voiceVolume);
        }
    }

    public void PlayHitReaction()
    {
        animator.SetLayerWeight(attackLayerIndex,1f);
        animator.SetTrigger(Random.value < 0.5f ? "Hit1" : "Hit2");
        hitStaggerTimer = hitStaggerDuration;
        agent.speed = currentBaseSpeed * hitSlowMultiplier;
        agent.velocity *= hitSlowMultiplier;

        PlayRandomClip(hurtSounds, voiceVolume);
        PlayRandomClip(impactSounds);
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
                PlayRandomClip(attackSounds);
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
            capsuleCollider.enabled = false;
            TryDropHealingPill();
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
        else
        {
            capsuleCollider.enabled=false;
            TryDropHealingPill();
        }
    }

    private void TryDropHealingPill()
    {
        if (healingPillPrefab == null) return;
        if (Random.value > healingPillDropChance) return;

        Instantiate(healingPillPrefab, transform.position, Quaternion.identity);
    }
    private void HandlePostDeathTransition()
    {
        if (awaitingRemoval)
        {
            corpseLingerTimer -= Time.deltaTime;
            if (corpseLingerTimer <= 0f)
            {
                gameObject.SetActive(false);
            }
            return;
        }
        
        if (frozenDeath)
        {
            deathFreezeTimer -= Time.deltaTime;
            if (deathFreezeTimer <= 0f)
            {
                awaitingRemoval = true;
                corpseLingerTimer = corpseLingerTime;
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
            awaitingRemoval = true;
            corpseLingerTimer = corpseLingerTime;
        }
    }
    public void PlayFootstep()
    {
        PlayRandomClip(footstepSounds);
    }

    public Transform GetBloodSprayPoint(Vector3 hitPoint)
    {
        float threshold = (headBone.position.y + bodyBone.position.y)/2f;
        return hitPoint.y > threshold ? headBone : bodyBone;
    }
}
