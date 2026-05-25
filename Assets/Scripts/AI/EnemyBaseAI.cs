using UnityEngine;
using UnityEngine.AI;
using static FleeState;

public class EnemyBaseAI : MonoBehaviour
{
    [Header("Тип поведения")]
    public bool isPeaceful = true; // ГАЛОЧКА: Мирный или нет

    [Header("Ссылки")]
    public NavMeshAgent agent;
    public Animator anim;
    public Transform player;
    public Health health;

    [Header("Настройки")]
    public float detectionRange = 15f;
    public float attackRange = 2f;

    [HideInInspector] public EnemyStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = gameObject.AddComponent<EnemyStateMachine>();

        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (health == null) health = GetComponent<Health>();
        if (player == null) player = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        // 1. ЛОГИКА СТАРТА
        if (isPeaceful)
        {
            // Мирный моб просто стоит в Idle
            stateMachine.Initialize(new IdleState(stateMachine, this));
        }
        else
        {
            // Агрессивный моб (дальник) сразу начинает искать игрока
            stateMachine.Initialize(new ChaseState(stateMachine, this));
        }
    }

    private void Update()
    {
        stateMachine.Update();
    }

    // Метод вызывается из Health.cs при получении урона
    public virtual void OnHit()
    {
        if (isPeaceful)
        {
            // ТЗ ПУНКТ 2: Если мирный моб получил урон и ХП мало — бегство
            if (health.currentHealth < health.maxHealth * 0.4f)
            {
                stateMachine.ChangeState(new FleeState(stateMachine, this));
            }
        }
        else
        {
            // Если моб агрессивный, он и так в Chase, но если был в Idle — агрим
            if (stateMachine.CurrentState is IdleState)
                stateMachine.ChangeState(new ChaseState(stateMachine, this));
        }
    }

    public void StartFleeing()
    {
        stateMachine.ChangeState(new FleeState(stateMachine, this));
    }
}