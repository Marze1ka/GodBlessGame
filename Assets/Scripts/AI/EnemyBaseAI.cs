using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseAI : MonoBehaviour
{
    [Header("Режим поведения")]
    public bool isPeaceful = true;
    public bool isRanged = false;

    [Header("Настройки ИИ")]
    public float detectionRange = 15f;
    public float attackRange = 3f;
    public float rangedStopDistance = 10f;

    [Header("Настройки задержки (Визуал)")]
    public float attackDelay = 0.5f;

    [Header("Ссылки на компоненты")]
    public NavMeshAgent agent;
    public Animator anim;
    public Transform player;
    public Health health;

    [Header("Ссылки на префабы")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [HideInInspector] public EnemyStateMachine stateMachine;

    protected virtual void Awake()
    {
        stateMachine = gameObject.AddComponent<EnemyStateMachine>();

        // ПРОВЕРКА: Если ссылка на игрока пустая (а в префабе она всегда пустая)
        if (player == null)
        {
            // Ищем на сцене объект с тегом Player
            GameObject playerObject = GameObject.FindWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("ОШИБКА: На сцене не найден объект с тегом Player! Проверь теги.");
            }
        }

        // Остальные ссылки (ищем их на самом себе)
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (health == null) health = GetComponent<Health>();
    }

    protected virtual void Start()
    {
        // Все мобы и боссы начинают в Покое
        stateMachine.Initialize(new IdleState(stateMachine, this));
    }

    private void Update()
    {
        stateMachine.Update();
    }

    // --- ЛОГИКА 1: БЛИЖНИЙ БОЙ (МЕЧ) ---
    public void StartMeleeAttackSequence()
    {
        Invoke("ExecuteMeleeLogic", attackDelay);
    }

    private void ExecuteMeleeLogic()
    {
        float finalDmg = 10f;

        // Если это Босс, берем его уникальный урон
        if (this is BossAI boss)
            finalDmg = boss.isUsingStrongMelee ? boss.strongAttackDamage : boss.weakAttackDamage;

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.ApplyDamage(finalDmg);
            Debug.Log("<color=white>УДАР МЕЧОМ!</color>");
        }
    }

    // --- ЛОГИКА 2: ДАЛЬНИЙ БОЙ (МАГИЯ) ---
    public void StartRangedAttackSequence()
    {
        Invoke("ExecuteRangedLogic", attackDelay);
    }

    private void ExecuteRangedLogic()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Целимся в грудь игрока
            Vector3 targetCenter = player.position + Vector3.up * 1.5f;
            Vector3 shootDir = (targetCenter - firePoint.position).normalized;

            GameObject ball = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDir));

            // Настройка урона для босса
            if (this is BossAI boss)
            {
                EnemyProjectile script = ball.GetComponent<EnemyProjectile>();
                if (script != null) script.damage = boss.currentMagicDamage;
            }
            Debug.Log("<color=cyan>ВЫСТРЕЛ МАГИЕЙ!</color>");
        }
    }

    public virtual void OnHit()
    {
        if (isPeaceful)
        {
            if (health.currentHealth < health.maxHealth * 0.4f)
                stateMachine.ChangeState(new FleeState(stateMachine, this));
        }
        else
        {
            if (stateMachine.CurrentState is IdleState)
                stateMachine.ChangeState(new ChaseState(stateMachine, this));
        }
    }

    public void StartFleeing() { stateMachine.ChangeState(new FleeState(stateMachine, this)); }
}
