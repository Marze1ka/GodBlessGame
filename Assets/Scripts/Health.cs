using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isPlayer = false;
    public Slider healthSlider;
    public Animator anim;
    public PlayerMovement movement; // Ссылка на движение (чтобы остановить труп)

    public float hitStunTime = 0.3f;
    public float deathDelay = 3.0f; // Сколько секунд ждем перед респауном

    private bool isDead = false; // Чтобы не умирать дважды

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null) { healthSlider.maxValue = maxHealth; healthSlider.value = currentHealth; }
    }

    public void TakeDamage(float amount, DamageType type)
    {
        if (isDead) return; // Мертвые урон не получают

        currentHealth -= amount;
        if (healthSlider != null) healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (isPlayer)
        {
            StartCoroutine(HitStun());
        }
    }

    IEnumerator HitStun()
    {
        if (anim != null) anim.SetTrigger("Hit");
        if (movement != null) movement.canMove = false;
        yield return new WaitForSeconds(hitStunTime);
        if (movement != null) movement.canMove = true;
    }

    void Die()
    {
        isDead = true;

        if (isPlayer)
        {
            StartCoroutine(PlayerDeathRoutine());
        }
        else
        {
            // Для моба можно просто запустить анимацию и удалить через 2 сек
            if (anim != null) anim.SetTrigger("Death");
            Destroy(gameObject, 2.0f);
        }
    }

    IEnumerator PlayerDeathRoutine()
    {
        Debug.Log("Игрок погиб...");

        if (anim != null) anim.SetTrigger("Death"); // 1. Запускаем анимацию падения
        if (movement != null) movement.canMove = false; // 2. Выключаем управление

        // 3. Выключаем физику (чтобы мобы не толкали труп)
        if (GetComponent<CharacterController>() != null)
            GetComponent<CharacterController>().enabled = false;

        yield return new WaitForSeconds(deathDelay); // 4. Ждем (например, 3 секунды)

        // 5. ПЕРЕЗАГРУЗКА
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}