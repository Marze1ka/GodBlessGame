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
        // ѕровер€ем, попали ли мы в игрока (через его новый контроллер)
        PlayerController pc = other.GetComponent<PlayerController>();

        if (pc != null)
        {
            pc.ApplyDamage(damage); // Ќаносим урон игроку
            Destroy(gameObject);    // ”дал€ем снар€д
        }

        // ≈сли попали не в игрока, а в стену или преп€тствие
        else if (other.gameObject.layer == 0) // Layer 0 - это Default (стены/пол)
        {
            // ≈сли это не моб (чтобы снар€ды не взрывались об самих мобов)
            if (other.GetComponent<Health>() == null)
            {
                Destroy(gameObject);
            }
        }
    }
}