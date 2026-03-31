using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim; // Ссылка на аниматор

    [Header("Настройки")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float damage = 10f;
    public float attackCooldown = 2f;
    float lastAttackTime;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. ПЕРЕДАЕМ СКОРОСТЬ В АНИМАТОР
        // agent.velocity.magnitude — это реальная скорость движения моба
        if (anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        agent.SetDestination(transform.position); // Остановиться

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // 2. ЗАПУСКАЕМ АНИМАЦИЮ АТАКИ
            if (anim != null) anim.SetTrigger("Attack");

            // Наносим урон (можно с задержкой через Invoke, если анимация длинная)
            Invoke("DealDamage", 0.5f);

            lastAttackTime = Time.time;
        }
    }

    void DealDamage()
    {
        // Проверяем еще раз дистанцию перед нанесением урона
        if (Vector3.Distance(transform.position, player.position) <= attackRange + 0.5f)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null) playerHealth.TakeDamage(damage, DamageType.Physical);
        }
    }
}