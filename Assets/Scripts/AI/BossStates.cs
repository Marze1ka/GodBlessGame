using UnityEngine;

// 1. ПОКОЙ
public class BossIdleState : EnemyState
{
    public BossIdleState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }
    public override void Enter() { if (context.agent.isOnNavMesh) context.agent.isStopped = true; context.anim.SetFloat("Speed", 0); }
}

// 2. ПОГОНЯ
public class BossChaseState : EnemyState
{
    public BossChaseState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }
    public override void Update()
    {
        if (!context.agent.enabled || !context.agent.isOnNavMesh) return;
        context.agent.isStopped = false;
        context.agent.SetDestination(context.player.position);
        context.anim.SetFloat("Speed", context.agent.velocity.magnitude);

        if (Vector3.Distance(context.transform.position, context.player.position) <= context.attackRange)
        {
            // Берем ссылку на настройки Босса
            BossAI boss = (BossAI)context;

            // С шансом 20% сильная атака, иначе обычная
            if (Random.value < 0.2f)
                stateMachine.ChangeState(new BossPowerAttackState(stateMachine, context));
            else
                stateMachine.ChangeState(new BossAttackState(stateMachine, context));
        }
    }
}

// 3. ОБЫЧНАЯ АТАКА
public class BossAttackState : EnemyState
{
    private float lastTime;
    public BossAttackState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }
    public override void Enter() { if (context.agent.isOnNavMesh) context.agent.isStopped = true; }
    public override void Update()
    {
        if (Time.time > lastTime + 2f)
        {
            context.anim.SetTrigger("Attack");
            PlayerController pc = context.player.GetComponent<PlayerController>();
            if (pc != null) pc.ApplyDamage(((BossAI)context).normalDamage);
            lastTime = Time.time;
        }
        if (Vector3.Distance(context.transform.position, context.player.position) > context.attackRange + 1f)
            stateMachine.ChangeState(new BossChaseState(stateMachine, context));
    }
}

// 4. СИЛЬНАЯ АТАКА (Добавили задержку перед следующим ударом)
public class BossPowerAttackState : EnemyState
{
    private float stateTimer;
    public BossPowerAttackState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }
    public override void Enter()
    {
        stateTimer = 0;
        if (context.agent.isOnNavMesh) context.agent.isStopped = true;
        context.anim.SetTrigger("PowerAttack");

        PlayerController pc = context.player.GetComponent<PlayerController>();
        if (pc != null) pc.ApplyDamage(((BossAI)context).powerDamage);
    }
    public override void Update()
    {
        stateTimer += Time.deltaTime;
        // Ждем 2 секунды, прежде чем Босс снова сможет бежать или атаковать
        if (stateTimer >= 2f)
        {
            stateMachine.ChangeState(new BossChaseState(stateMachine, context));
        }
    }
}