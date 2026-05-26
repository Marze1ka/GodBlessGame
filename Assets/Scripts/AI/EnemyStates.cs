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
        // ПРОВЕРКА: Если галочка "Мирный" СНЯТА (моб агрессивный)
        if (!context.isPeaceful)
        {
            float distance = Vector3.Distance(context.transform.position, context.player.position);

            // Только если мы НЕ мирные, мы переходим в погоню
            if (distance <= context.detectionRange)
            {
                Debug.Log(context.gameObject.name + " (Агрессивный) заметил игрока!");
                stateMachine.ChangeState(new ChaseState(stateMachine, context));
            }
        }
        else
        {
            // Если моб мирный, он просто стоит в Idle. 
            // Мы здесь ничего не пишем, поэтому он никогда не перейдет в Chase сам.
        }
    }
}
// --- 2. АГРЕССИЯ / ПОГОНЯ (CHASE) ---
public class ChaseState : EnemyState
{
    public ChaseState(EnemyStateMachine machine, EnemyBaseAI context) : base(machine, context) { }

    public override void Update()
    {
        if (!context.agent.enabled || !context.agent.isOnNavMesh) return;

        float distance = Vector3.Distance(context.transform.position, context.player.position);

        // ЛОГИКА ДЛЯ ДАЛЬНИКА
        if (context.isRanged)
        {
            if (distance <= context.rangedStopDistance)
            {
                // Мы на дистанции выстрела — переходим к атаке
                stateMachine.ChangeState(new AttackState(stateMachine, context));
            }
            else
            {
                // Мы еще далеко — бежим к игроку
                context.agent.isStopped = false;
                context.agent.SetDestination(context.player.position);
                context.anim.SetFloat("Speed", context.agent.velocity.magnitude);
            }
        }
        // ЛОГИКА ДЛЯ БЛИЖНИКА
        else
        {
            context.agent.isStopped = false;
            context.agent.SetDestination(context.player.position);
            context.anim.SetFloat("Speed", context.agent.velocity.magnitude);

            if (distance <= context.attackRange)
            {
                stateMachine.ChangeState(new AttackState(stateMachine, context));
            }
        }
    }
}

// --- 3. АТАКА (ATTACK) ---
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
        // Поворот к игроку
        Vector3 dir = (context.player.position - context.transform.position).normalized;
        context.transform.rotation = Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)), Time.deltaTime * 5f);

        if (Time.time >= lastAttackTime + cooldown)
        {
            context.anim.SetTrigger("Attack");

            // ИСПРАВЛЕНИЕ: Выбираем нужную цепочку в зависимости от типа моба
            if (context.isRanged)
                context.StartRangedAttackSequence(); // Для магов
            else
                context.StartMeleeAttackSequence();  // Для воинов

            lastAttackTime = Time.time;
        }

        // Выход из состояния
        float distance = Vector3.Distance(context.transform.position, context.player.position);
        if (distance > context.attackRange + 0.5f)
        {
            stateMachine.ChangeState(new ChaseState(stateMachine, context));
        }
    }

    public override void Exit()
    {
        // Если моб передумал атаковать (ты убежал), отменяем выстрел
        context.CancelInvoke("SpawnProjectileLogic");
    }
}






// НОВАЯ ФУНКЦИЯ ЗАДЕРЖКИ


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
