using UnityEngine;
using UnityEngine.AI; // Обязательно для работы с NavMesh

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;     // Ссылка на компонент ног
    public Transform player;       // Ссылка на трансформ игрока

    [Header("Настройки ИИ")]
    public float detectionRange = 10f; // Радиус обнаружения
    public float attackRange = 2f;    // Радиус атаки
    public float damage = 10f;        // Урон игроку
    public float attackCooldown = 2f; // Перезарядка удара
    float lastAttackTime;             // Время последнего удара

    void Start()
    {
        // Если забыли перетащить игрока вручную, попробуем найти его по тегу
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // 1. Считаем дистанцию между мобом и игроком
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 2. Логика состояний
        if (distanceToPlayer <= attackRange)
        {
            // СОСТОЯНИЕ: АТАКА
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // СОСТОЯНИЕ: ПРЕСЛЕДОВАНИЕ
            ChasePlayer();
        }
        else
        {
            // СОСТОЯНИЕ: ОЖИДАНИЕ
            StopEnemy();
        }
    }

    void ChasePlayer()
    {
        // Даем команду агенту: «Иди к координатам игрока»
        agent.SetDestination(player.position);
    }

    void StopEnemy()
    {
        // Даем команду агенту: «Иди туда, где ты сейчас стоишь» (остановиться)
        agent.SetDestination(transform.position);
    }

    void AttackPlayer()
    {
        // Останавливаемся перед ударом
        agent.SetDestination(transform.position);

        // Проверяем, прошла ли перезарядка
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Моб ударил игрока!");

            // Наносим урон через скрипт Health, который мы писали ранее
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, DamageType.Physical);
            }

            lastAttackTime = Time.time; // Запоминаем время удара
        }
    }

    // ВИЗУАЛИЗАЦИЯ: Чтобы мы видели радиусы в окне Scene
    private void OnDrawGizmosSelected()
    {
        // Рисуем синий круг для радиуса обнаружения
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Рисуем красный круг для радиуса атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}