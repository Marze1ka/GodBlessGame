using UnityEngine;

// 1. ПОКОЙ БОССА
public class BossIdleState : EnemyState
{
    public BossIdleState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }
    public override void Enter()
    {
        if (context.agent.isOnNavMesh) context.agent.isStopped = true;
        context.anim.SetFloat("Speed", 0);
    }
}

// 2. ПОГОНЯ (ВЫБОР ЗОНЫ)
public class BossChaseState : EnemyState
{
    public BossChaseState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Update()
    {
        if (!context.agent.enabled || !context.agent.isOnNavMesh) return;

        float distance = Vector3.Distance(context.transform.position, context.player.position);
        BossAI boss = (BossAI)context;

        // ЗОНА 1: В упор (Меч)
        if (distance <= context.attackRange)
        {
            stateMachine.ChangeState(new BossMeleeAttackState(stateMachine, context));
        }
        // ЗОНА 2: Средняя (Магия)
        else if (distance <= boss.rangedDistance)
        {
            stateMachine.ChangeState(new BossRangedState(stateMachine, context));
        }
        // ЗОНА 3: Далеко (Бежим)
        else
        {
            context.agent.isStopped = false;
            context.agent.SetDestination(context.player.position);
            context.anim.SetFloat("Speed", context.agent.velocity.magnitude);
        }
    }
}

// 3. СОСТОЯНИЕ: БЛИЖНИЙ БОЙ
public class BossMeleeAttackState : EnemyState
{
    private float timer = 1.5f;
    public BossMeleeAttackState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Enter() { if (context.agent.isOnNavMesh) context.agent.isStopped = true; }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 2f)
        {
            bool isStrong = Random.value > 0.5f;
            ((BossAI)context).isUsingStrongMelee = isStrong;

            // Только триггеры меча
            context.anim.SetTrigger(isStrong ? "PowerAttack" : "Attack");
            context.StartMeleeAttackSequence();
            timer = 0;
        }

        if (Vector3.Distance(context.transform.position, context.player.position) > context.attackRange + 0.5f)
            stateMachine.ChangeState(new BossChaseState(stateMachine, context));
    }

    public override void Exit() { context.CancelInvoke("ExecuteMeleeLogic"); }
}

// 4. СОСТОЯНИЕ: ДАЛЬНИЙ БОЙ
public class BossRangedState : EnemyState
{
    private float timer = 2f;
    public BossRangedState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Enter() { if (context.agent.isOnNavMesh) context.agent.isStopped = true; context.anim.SetFloat("Speed", 0); }

    public override void Update()
    {
        // Поворот к игроку
        Vector3 dir = (context.player.position - context.transform.position).normalized;
        dir.y = 0;
        context.transform.rotation = Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

        timer += Time.deltaTime;
        if (timer >= 3f)
        {
            // ТОЛЬКО триггер магии
            context.anim.SetTrigger("Cast");
            context.StartRangedAttackSequence();
            timer = 0;
        }

        float dist = Vector3.Distance(context.transform.position, context.player.position);
        if (dist < context.attackRange - 0.5f || dist > ((BossAI)context).rangedDistance)
            stateMachine.ChangeState(new BossChaseState(stateMachine, context));
    }

    public override void Exit() { context.CancelInvoke("ExecuteRangedLogic"); }
}
