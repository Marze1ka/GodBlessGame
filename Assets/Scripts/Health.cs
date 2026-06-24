using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Параметры здоровья")]
    public float maxHealth = 100f;
    public float currentHealth;
    [FormerlySerializedAs("Player")] public bool playerControlled;

    [Header("Ссылки")]
    public Slider healthSlider;
    public Animator anim;

    private bool dead;

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
        if (dead) return;

        currentHealth -= amount;
        if (healthSlider != null) healthSlider.value = currentHealth;

        EnemyBaseAI ai = GetComponent<EnemyBaseAI>();
        if (ai != null) ai.OnHit();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        EnemyBaseAI ai = GetComponent<EnemyBaseAI>();
        if (ai != null)
        {
            ai.stateMachine.ChangeState(new DeathState(ai.stateMachine, ai));
        }

        if (!playerControlled)
        {
            if (ScoreboardManager.Instance != null)
            {
                ScoreboardManager.Instance.AddKill();
            }

            Destroy(gameObject, 3.0f);
        }
        else
        {
        }
    }
}
