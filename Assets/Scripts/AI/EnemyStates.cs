using UnityEngine;

public class IdleState : EnemyState
{
    public IdleState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

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

        if (!context.peacefulMode)
        {
            float distance = Vector3.Distance(context.transform.position, context.player.position);

            if (distance <= context.detectionRange)
            {
                Debug.Log(context.gameObject.name + " (Агрессивный) заметил игрока!");
                stateMachine.ChangeState(new ChaseState(stateMachine, context));
            }
        }
    }
}

public class ChaseState : EnemyState
{
    public ChaseState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Update()
    {
        if (!context.ResolvePlayerReference())
        {
            return;
        }

        if (!context.agent.enabled || !context.agent.isOnNavMesh) return;

        float distance = Vector3.Distance(context.transform.position, context.player.position);

        if (context.rangedCombat)
        {
            if (distance <= context.rangedStopDistance)
            {
                stateMachine.ChangeState(new AttackState(stateMachine, context));
            }
            else
            {
                context.agent.isStopped = false;
                context.agent.SetDestination(context.player.position);
                context.anim.SetFloat("Speed", context.agent.velocity.magnitude);
            }
        }
        else
        {
            context.agent.isStopped = false;
            context.agent.SetDestination(context.player.position);
            context.anim.SetFloat("Speed", context.agent.velocity.magnitude);

            if (distance <= context.GetEffectiveMeleeAttackDistance())
            {
                stateMachine.ChangeState(new AttackState(stateMachine, context));
            }
        }
    }
}

public class AttackState : EnemyState
{
    private float lastAttackTime;
    private float cooldown = 3f;

    public AttackState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

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
        context.transform.rotation = Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)), Time.deltaTime * 5f);

        if (Time.time >= lastAttackTime + cooldown)
        {
            context.anim.SetTrigger("Attack");

            if (context.rangedCombat)
            {
                context.StartRangedAttackSequence();
            }
            else
            {
                context.StartMeleeAttackSequence();
            }

            lastAttackTime = Time.time;
        }

        float distance = Vector3.Distance(context.transform.position, context.player.position);
        float maxAttackDistance = context.rangedCombat ? context.rangedStopDistance : context.GetEffectiveMeleeAttackDistance();
        if (distance > maxAttackDistance + 0.5f)
        {
            stateMachine.ChangeState(new ChaseState(stateMachine, context));
        }
    }

    public override void Exit()
    {
        context.CancelPendingRangedAttack();
    }
}

public class FleeState : EnemyState
{
    public FleeState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Update()
    {
        if (!context.ResolvePlayerReference())
        {
            return;
        }

        if (!context.agent.enabled || !context.agent.isOnNavMesh) return;

        Vector3 runDir = context.transform.position - context.player.position;
        Vector3 dest = context.transform.position + runDir.normalized * 5f;

        context.agent.isStopped = false;
        context.agent.SetDestination(dest);
        context.anim.SetFloat("Speed", context.agent.velocity.magnitude);
    }
}

public class DeathState : EnemyState
{
    public DeathState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Enter()
    {
        if (context.agent.isOnNavMesh) context.agent.isStopped = true;
        context.agent.enabled = false;
        context.anim.SetTrigger("Death");
    }

    public override void Update()
    {
    }
}
