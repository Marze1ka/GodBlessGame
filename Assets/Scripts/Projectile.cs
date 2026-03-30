using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 15f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // Удалить через 3 сек, если никуда не попал
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null && !health.isPlayer) // Не раним самого себя
        {
            health.TakeDamage(damage, DamageType.Magical);
            Destroy(gameObject); // Исчезаем при попадании
        }
    }
}