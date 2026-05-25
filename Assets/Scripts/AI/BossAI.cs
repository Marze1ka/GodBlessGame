using UnityEngine;

public class BossAI : EnemyBaseAI
{
    [Header("Настройки урона Босса")]
    public float normalDamage = 10f; // Урон обычной атаки
    public float powerDamage = 25f;  // Урон сильной атаки

    [Header("Фаза 2")]
    public float phaseTwoThreshold = 0.5f;
    private bool isEnraged = false;

    private void Start()
    {
        stateMachine.Initialize(new BossIdleState(stateMachine, this));
    }

    private void Update()
    {
        stateMachine.Update();

        if (!isEnraged && health != null && health.currentHealth < health.maxHealth * phaseTwoThreshold)
        {
            isEnraged = true;
            anim.speed = 1.5f;
            agent.speed *= 1.3f;
            // Увеличиваем урон во второй фазе (опционально)
            normalDamage *= 1.2f;
            powerDamage *= 1.2f;
        }
    }

    public override void OnHit()
    {
        if (stateMachine.CurrentState is BossIdleState)
        {
            stateMachine.ChangeState(new BossChaseState(stateMachine, this));
        }
    }
}