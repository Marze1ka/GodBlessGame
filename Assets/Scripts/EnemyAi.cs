using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim; 

    [Header("Настройки")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float damage = 10f;
    public float attackCooldown = 2f;
    float lastAttackTime;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
        agent.SetDestination(transform.position); 

        if (Time.time >= lastAttackTime + attackCooldown)
        {   
            if (anim != null) anim.SetTrigger("Attack");

            Invoke("DealDamage", 0.5f);

            lastAttackTime = Time.time;
        }
    }

    void DealDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange + 0.5f)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null) playerHealth.TakeDamage(damage, DamageType.Physical);
        }
    }
}