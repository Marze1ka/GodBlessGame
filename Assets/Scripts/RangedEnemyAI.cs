using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;

    [Header("Дистанции")]
    public float detectionRange = 15f; 
    public float stopDistance = 8f;    
    public float attackRange = 10f;   

    [Header("Атака")]
    public GameObject projectilePrefab;
    public Transform firePoint; 
    public float attackCooldown = 3f;
    float lastAttackTime;
    public float spawnDelay = 1f; 
    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.stoppingDistance = stopDistance;
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh || !agent.enabled)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (anim != null) anim.SetFloat("Speed", agent.velocity.magnitude);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > stopDistance)
            {
                agent.SetDestination(player.position);
            }
            else
            {            
                if (agent.isOnNavMesh && agent.enabled)
                    agent.ResetPath();

                FacePlayer();

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

        Invoke("SpawnProjectile", spawnDelay);
    }

    void SpawnProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, transform.rotation);
        }
    }
}