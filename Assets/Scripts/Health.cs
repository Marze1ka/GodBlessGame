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

    [Header("Ссылки на UI и Анимации")]
    public Slider healthSlider; 
    public Animator anim;         
    public PlayerMovement movement; 

    [Header("Настройки таймингов")]
    public float hitStunTime = 0.4f; 
    public float deathDelay = 3.0f;  

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

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        Debug.Log(gameObject.name + " получил " + amount + " " + type + " урона");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HitStunRoutine());
        }
    }

    IEnumerator HitStunRoutine()
    {
        if (anim != null) anim.SetTrigger("Hit"); 

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

    IEnumerator PlayerDeathRoutine()
    {
        if (anim != null) anim.SetTrigger("Death");
        if (movement != null) movement.canMove = false;

        // ИСПРАВЛЕНО: Проверяем, есть ли компонент, прежде чем отключать
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        yield return new WaitForSeconds(deathDelay);

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null) gm.ShowGameOverScreen();
    }

    void EnemyDeath()
    {
        if (anim != null) anim.SetTrigger("Death");
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        MonoBehaviour aiScript = GetComponent<EnemyAI>(); 
        if (aiScript != null) aiScript.enabled = false;

        Destroy(gameObject, 3.0f);
    }
}