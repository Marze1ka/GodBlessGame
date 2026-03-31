using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;

    [Header("Дистанции")]
    public float detectionRange = 15f; // Когда заметит
    public float stopDistance = 8f;    // На каком расстоянии остановится, чтобы стрелять
    public float attackRange = 10f;   // Дальность полета магии

    [Header("Атака")]
    public GameObject projectilePrefab;
    public Transform firePoint; // Точка в руке
    public float attackCooldown = 3f;
    float lastAttackTime;
    public float spawnDelay = 1f; // Через сколько секунд после начала анимации вылетит шар
    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        // Устанавливаем дистанцию остановки в NavMesh
        agent.stoppingDistance = stopDistance;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Передаем скорость в аниматор
        if (anim != null) anim.SetFloat("Speed", agent.velocity.magnitude);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > stopDistance)
            {
                // Идем к игроку
                agent.SetDestination(player.position);
            }
            else
            {
                // Мы на месте — стоим и атакуем
                agent.ResetPath();
                FacePlayer(); // Поворачиваемся лицом к игроку

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
        }
    }

    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        if (anim != null) anim.SetTrigger("Cast");

        // Теперь мы используем переменную вместо жесткого числа 0.5
        Invoke("SpawnProjectile", spawnDelay);
    }

    void SpawnProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Создаем снаряд и направляем его в сторону игрока
            Instantiate(projectilePrefab, firePoint.position, transform.rotation);
        }
    }
}