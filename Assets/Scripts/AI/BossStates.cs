using UnityEngine;

public class BossIdleState : EnemyState
{
    public BossIdleState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Enter()
    {
        if (context.agent.isOnNavMesh) context.agent.isStopped = true;
        context.anim.SetFloat("Speed", 0);
    }

    public override void Update()
    {
        if (context.peacefulMode || !context.ResolvePlayerReference())
        {
            return;
        }

        float distance = Vector3.Distance(context.transform.position, context.player.position);
        if (distance <= context.detectionRange)
        {
            stateMachine.ChangeState(new BossChaseState(stateMachine, context));
        }
    }
}

public class BossChaseState : EnemyState
{
    public BossChaseState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Update()
    {
        if (!context.ResolvePlayerReference())
        {
            return;
        }

        if (!context.agent.enabled || !context.agent.isOnNavMesh) return;

        float distance = Vector3.Distance(context.transform.position, context.player.position);
        BossAI boss = (BossAI)context;

        if (distance <= context.attackRange)
        {
            stateMachine.ChangeState(new BossMeleeAttackState(stateMachine, context));
        }
        else if (distance <= boss.RangedDistance)
        {
            stateMachine.ChangeState(new BossRangedState(stateMachine, context));
        }
        else
        {
            context.agent.isStopped = false;
            context.agent.SetDestination(context.player.position);
            context.anim.SetFloat("Speed", context.agent.velocity.magnitude);
        }
    }
}

public class BossMeleeAttackState : EnemyState
{
    private float timer = 1.5f;

    public BossMeleeAttackState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Enter()
    {
        if (context.agent.isOnNavMesh) context.agent.isStopped = true;
    }

    public override void Update()
    {
        if (!context.ResolvePlayerReference())
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= 2f)
        {
            BossAI boss = (BossAI)context;
            boss.SelectMeleeAttackType();

            context.StartMeleeAttackSequence();
            context.anim.SetTrigger("Attack");
            timer = 0;
        }

        if (Vector3.Distance(context.transform.position, context.player.position) > context.attackRange + 0.5f)
        {
            stateMachine.ChangeState(new BossChaseState(stateMachine, context));
        }
    }

    public override void Exit()
    {
        // Если удар уже начался, не отменяем его при смене состояния.
    }
}

public class BossRangedState : EnemyState
{
    private float timer = 2f;

    public BossRangedState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Enter()
    {
        if (context.agent.isOnNavMesh) context.agent.isStopped = true;
        context.anim.SetFloat("Speed", 0);
    }

    public override void Update()
    {
        if (!context.ResolvePlayerReference())
        {
            return;
        }

        Vector3 dir = (context.player.position - context.transform.position).normalized;
        dir.y = 0;
        context.transform.rotation = Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

        timer += Time.deltaTime;
        if (timer >= 3f)
        {
            context.anim.SetTrigger("Cast");
            context.StartRangedAttackSequence();
            timer = 0;
        }

        float dist = Vector3.Distance(context.transform.position, context.player.position);
        if (dist < context.attackRange - 0.5f || dist > ((BossAI)context).RangedDistance)
        {
            stateMachine.ChangeState(new BossChaseState(stateMachine, context));
        }
    }

    public override void Exit()
    {
        context.CancelPendingRangedAttack();
    }
}
