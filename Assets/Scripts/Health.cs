using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Параметры здоровья")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isPlayer = false;

    [Header("Ссылки")]
    public Slider healthSlider;
    public Animator anim;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount, DamageType type)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (healthSlider != null) healthSlider.value = currentHealth;

        // ПИНАЕМ ИИ ПРИ УДАРЕ (Мирный режим)
        EnemyBaseAI ai = GetComponent<EnemyBaseAI>();
        if (ai != null) ai.OnHit();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Пытаемся найти ИИ, чтобы переключить в состояние смерти
        EnemyBaseAI ai = GetComponent<EnemyBaseAI>();
        if (ai != null)
        {
            ai.stateMachine.ChangeState(new DeathState(ai.stateMachine, ai));
        }

        // ЛАБА 7: Начисляем очки, если умер НЕ ИГРОК
        if (!isPlayer)
        {
            if (ScoreboardManager.Instance != null)
            {
                ScoreboardManager.Instance.AddKill();
            }

            Destroy(gameObject, 3.0f); // Удаляем моба через 3 сек
        }
        else
        {
            // Логика смерти игрока (экран рестарта), которую мы писали раньше
        }
    }
}