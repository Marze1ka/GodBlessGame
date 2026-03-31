using UnityEngine;
using UnityEngine.UI;           // Для работы с полоской здоровья (Slider)
using UnityEngine.SceneManagement; // Для перезагрузки сцены
using System.Collections;       // Для работы задержек (Coroutines)

public class Health : MonoBehaviour
{
    [Header("Параметры здоровья")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isPlayer = false;   // Поставь галочку, если этот скрипт на Игроке

    [Header("Ссылки на UI и Анимации")]
    public Slider healthSlider;     // Перетащи сюда слайдер из Canvas
    public Animator anim;           // Перетащи сюда модель с аниматором
    public PlayerMovement movement; // Ссылка на скрипт движения (только для игрока)

    [Header("Настройки таймингов")]
    public float hitStunTime = 0.4f; // Сколько секунд нельзя ходить при ударе
    public float deathDelay = 3.0f;  // Сколько ждать перед респауном игрока

    private bool isDead = false;    // Флаг, чтобы не умирать дважды

    void Start()
    {
        currentHealth = maxHealth;

        // Настройка слайдера при старте
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // МЕТОД ПОЛУЧЕНИЯ УРОНА (вызывается из PlayerCombat или EnemyAI)
    public void TakeDamage(float amount, DamageType type)
    {
        if (isDead) return; // Если уже мертв — ничего не делаем

        currentHealth -= amount;

        // Обновляем полоску визуально
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        Debug.Log(gameObject.name + " получил " + amount + " " + type + " урона.");

        // Проверяем на смерть
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Если выжил — запускаем "реакцию на удар" (Hit Stun)
            StartCoroutine(HitStunRoutine());
        }
    }

    // КОРУТИНА: Микро-стан при получении урона
    IEnumerator HitStunRoutine()
    {
        if (anim != null) anim.SetTrigger("Hit"); // Запуск анимации вздрагивания

        // Если это игрок — запрещаем ему ходить на время стана
        if (isPlayer && movement != null)
        {
            movement.canMove = false;
            yield return new WaitForSeconds(hitStunTime);
            movement.canMove = true;
        }
        else
        {
            yield return null;
        }
    }

    // МЕТОД СМЕРТИ
    void Die()
    {
        isDead = true;

        if (isPlayer)
        {
            StartCoroutine(PlayerDeathRoutine());
        }
        else
        {
            EnemyDeath();
        }
    }

    // ЛОГИКА СМЕРТИ ИГРОКА
    IEnumerator PlayerDeathRoutine()
    {
        Debug.Log("Игрок падает...");

        if (anim != null) anim.SetTrigger("Death");
        if (movement != null) movement.canMove = false;

        // Выключаем коллайдер, чтобы мобы не толкали труп
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Ждем, пока доиграется анимация падения (например, 3 секунды)
        yield return new WaitForSeconds(deathDelay);

        // Находим GameManager на сцене и просим его показать UI
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.ShowGameOverScreen();
        }
    }

    // ЛОГИКА СМЕРТИ МОБА
    void EnemyDeath()
    {
        if (anim != null) anim.SetTrigger("Death");

        // Выключаем его ИИ и навигацию, чтобы он не ходил мертвым
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        MonoBehaviour aiScript = GetComponent<EnemyAI>(); // Предположим, скрипт ИИ так называется
        if (aiScript != null) aiScript.enabled = false;

        // Удаляем объект моба со сцены через 3 секунды (чтобы анимация успела проиграться)
        Destroy(gameObject, 3.0f);
    }
}