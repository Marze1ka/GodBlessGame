using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyBaseAI : MonoBehaviour
{
    [Header("Behavior")]
    [FormerlySerializedAs("isPeaceful")] public bool peacefulMode = true;
    [FormerlySerializedAs("isRanged")] public bool rangedCombat;

    [Header("AI Settings")]
    public float detectionRange = 15f;
    public float attackRange = 3f;
    public float rangedStopDistance = 10f;

    [Header("Visual Delay")]
    public float attackDelay = 0.5f;

    [Header("Components")]
    public NavMeshAgent agent;
    public Animator anim;
    public Transform player;
    public Health health;

    [Header("Weapon Data")]
    public Transform weaponSocket;
    public EnemyWeaponData currentData;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [HideInInspector] public EnemyStateMachine stateMachine;
    private Coroutine _pendingMeleeRoutine;
    private Coroutine _pendingRangedRoutine;

    protected virtual void Awake()
    {
        stateMachine = GetComponent<EnemyStateMachine>();
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<EnemyStateMachine>();
        }

        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (health == null) health = GetComponent<Health>();

        ResolvePlayerReference();
    }

    protected virtual void Start()
    {
        stateMachine.Initialize(CreateInitialState());
    }

    private void Update()
    {
        stateMachine.Update();
    }

    protected virtual EnemyState CreateInitialState()
    {
        return new IdleState(stateMachine, this);
    }

    public void StartMeleeAttackSequence()
    {
        CancelPendingMeleeAttack();
        _pendingMeleeRoutine = StartCoroutine(DelayedMeleeRoutine());
    }

    private IEnumerator DelayedMeleeRoutine()
    {
        yield return new WaitForSeconds(attackDelay);
        _pendingMeleeRoutine = null;
        ExecuteMeleeLogic();
    }

    protected virtual void ExecuteMeleeLogic()
    {
        PlayerController playerController = ResolvePlayerController();
        if (playerController == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);
        if (distanceToPlayer > GetEffectiveMeleeAttackDistance() + 1f)
        {
            return;
        }

        playerController.ApplyDamage(GetMeleeDamage());
        Debug.Log("<color=white>ENEMY MELEE HIT</color>");
    }

    public void StartRangedAttackSequence()
    {
        CancelPendingRangedAttack();
        _pendingRangedRoutine = StartCoroutine(DelayedRangedRoutine());
    }

    private IEnumerator DelayedRangedRoutine()
    {
        yield return new WaitForSeconds(attackDelay);
        _pendingRangedRoutine = null;
        ExecuteRangedLogic();
    }

    protected virtual void ExecuteRangedLogic()
    {
        if (!ResolvePlayerReference())
        {
            return;
        }

        if (projectilePrefab != null && firePoint != null)
        {
            Vector3 targetCenter = player.position + Vector3.up * 1.5f;
            Vector3 shootDir = (targetCenter - firePoint.position).normalized;

            GameObject ball = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDir));
            ConfigureProjectile(ball);
            Debug.Log("<color=cyan>ENEMY RANGED HIT</color>");
        }
    }

    public virtual void OnHit()
    {
        if (peacefulMode)
        {
            if (health.currentHealth < health.maxHealth * 0.4f)
            {
                stateMachine.ChangeState(new FleeState(stateMachine, this));
            }
        }
        else
        {
            EnterCombatState();
        }
    }

    protected virtual void EnterCombatState()
    {
        stateMachine.ChangeState(new ChaseState(stateMachine, this));
    }

    protected virtual float GetMeleeDamage()
    {
        return currentData != null ? currentData.damage : 10f;
    }

    public float GetEffectiveMeleeAttackDistance()
    {
        float agentReach = agent != null ? agent.stoppingDistance + agent.radius + 0.75f : 0f;
        return Mathf.Max(attackRange, agentReach);
    }

    protected virtual void ConfigureProjectile(GameObject projectileObject)
    {
    }

    public void StartFleeing()
    {
        stateMachine.ChangeState(new FleeState(stateMachine, this));
    }

    public void CancelPendingMeleeAttack()
    {
        if (_pendingMeleeRoutine != null)
        {
            StopCoroutine(_pendingMeleeRoutine);
            _pendingMeleeRoutine = null;
        }
    }

    public void CancelPendingRangedAttack()
    {
        if (_pendingRangedRoutine != null)
        {
            StopCoroutine(_pendingRangedRoutine);
            _pendingRangedRoutine = null;
        }
    }

    public bool ResolvePlayerReference()
    {
        if (player != null)
        {
            return true;
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null)
        {
            PlayerController playerController = Object.FindAnyObjectByType<PlayerController>();
            if (playerController != null)
            {
                playerObject = playerController.gameObject;
            }
        }

        if (playerObject == null)
        {
            return false;
        }

        player = playerObject.transform;
        return true;
    }

    protected PlayerController ResolvePlayerController()
    {
        if (ResolvePlayerReference())
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                return playerController;
            }
        }

        return Object.FindAnyObjectByType<PlayerController>();
    }
}
