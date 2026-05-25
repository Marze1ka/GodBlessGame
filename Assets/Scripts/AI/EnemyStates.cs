using UnityEngine;

// --- 1. ПОКОЙ (IDLE) ---
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
        // В мирном режиме мы ничего не делаем. 
        // Переход в Chase произойдет только через OnHit в скрипте EnemyBaseAI.
    }
}

// --- 2. АГРЕССИЯ / ПОГОНЯ (CHASE) ---
public class ChaseState : EnemyState
{
    public ChaseState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Update()
    {
        if (!context.agent.enabled || !context.agent.isOnNavMesh) return;

        context.agent.isStopped = false;
        context.agent.SetDestination(context.player.position);
        context.anim.SetFloat("Speed", context.agent.velocity.magnitude);

        float distance = Vector3.Distance(context.transform.position, context.player.position);
        if (distance <= context.attackRange)
        {
            stateMachine.ChangeState(new AttackState(stateMachine, context));
        }
    }
}

// --- 3. АТАКА (ATTACK) ---
public class AttackState : EnemyState
{
    private float lastAttackTime;
    private float cooldown = 2f;

    public AttackState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Enter()
    {
        if (context.agent.isOnNavMesh) context.agent.isStopped = true;
        context.anim.SetFloat("Speed", 0);
    }

    public override void Update()
    {
        // Поворот к игроку
        Vector3 dir = (context.player.position - context.transform.position).normalized;
        context.transform.rotation = Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)), Time.deltaTime * 5f);

        if (Time.time >= lastAttackTime + cooldown)
        {
            context.anim.SetTrigger("Attack");

            // УРОН ПО ИГРОКУ (MVC)
            PlayerController pc = context.player.GetComponent<PlayerController>();
            if (pc != null) pc.ApplyDamage(10f);

            lastAttackTime = Time.time;
        }

        if (Vector3.Distance(context.transform.position, context.player.position) > context.attackRange + 0.5f)
        {
            stateMachine.ChangeState(new ChaseState(stateMachine, context));
        }
    }
}

// --- 4. БЕГСТВО (FLEE) ---
public class FleeState : EnemyState
{
    public FleeState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Update()
    {
        if (!context.agent.enabled || !context.agent.isOnNavMesh) return;

        Vector3 runDir = context.transform.position - context.player.position;
        Vector3 dest = context.transform.position + runDir.normalized * 5f;

        context.agent.isStopped = false;
        context.agent.SetDestination(dest);
        context.anim.SetFloat("Speed", context.agent.velocity.magnitude);
    }
}

// --- 5. СМЕРТЬ (DEATH) - Чтобы не было ошибок NavMesh ---
public class DeathState : EnemyState
{
    public DeathState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Enter()
    {
        if (context.agent.isOnNavMesh) context.agent.isStopped = true;
        context.agent.enabled = false; // ВЫКЛЮЧАЕМ НОГИ
        context.anim.SetTrigger("Death");
    }
    public override void Update() { /* В смерти мозг не работает */ }
}
