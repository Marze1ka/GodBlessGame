using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 10f;
    public float lifeTime = 3f;

    void Start() { Destroy(gameObject, lifeTime); }

    void Update() { transform.Translate(Vector3.forward * speed * Time.deltaTime); }

    private void OnTriggerEnter(Collider other)
    {
        Health targetHealth = other.GetComponent<Health>();

        // Магия моба должна бить ТОЛЬКО игрока
        if (targetHealth != null && targetHealth.isPlayer)
        {
            targetHealth.TakeDamage(damage, DamageType.Magical);
            Destroy(gameObject);
        }
    }
}