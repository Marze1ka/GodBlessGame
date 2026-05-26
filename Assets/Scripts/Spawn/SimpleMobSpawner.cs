using UnityEngine;

public class SimpleMobSpawner : MonoBehaviour
{
    [Header("Кого спавним?")]
    public GameObject mobPrefab; // Сюда перетаскиваем префаб моба

    [Header("Настройки")]
    public bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        if (mobPrefab == null)
        {
            Debug.LogWarning("Спавнер на объекте " + gameObject.name + " пустой! Забыли префаб.");
            return;
        }

        // Создаем моба ровно в позиции этого спавнера
        Instantiate(mobPrefab, transform.position, transform.rotation);
    }

    // Рисуем иконку в окне Scene, чтобы спавнеры было видно
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f); // Маленькая зеленая сфера на месте спавна
    }
}