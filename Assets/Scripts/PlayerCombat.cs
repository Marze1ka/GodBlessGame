using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ссылки на компоненты")]
    public PlayerMovement movement;   
    public Animator anim;             

    [Header("Настройки урона")]
    public float physDamage = 20f;     
    public float magicDamage = 15f;    
    public float attackRadius = 1.5f;  

    [Header("Точки атаки")]
    public Transform swordPoint;       
    public Transform firePoint;        
    public GameObject magicProjectilePrefab; 

    [Header("Магия и Кулдаун")]
    public Slider magicSlider;         
    public float magicCooldown = 3f;   
    private float magicTimer;          
    private bool isMagicReady = true;  

    [Header("Тайминги анимаций")]
    public float attackLockTime = 0.8f; 
    public float magicLockTime = 1.2f;  

    void Start()
    {
        magicTimer = magicCooldown;

        if (magicSlider != null)
        {
            magicSlider.maxValue = magicCooldown;
            magicSlider.value = magicCooldown;
            magicSlider.fillRect.GetComponent<Image>().color = Color.cyan;
        }
    }

    void Update()
    {
        if (!isMagicReady)
        {
            magicTimer += Time.deltaTime;

            if (magicSlider != null)
                magicSlider.value = magicTimer;

            if (magicTimer >= magicCooldown)
            {
                isMagicReady = true;
                magicTimer = magicCooldown;
                if (magicSlider != null)
                    magicSlider.fillRect.GetComponent<Image>().color = Color.cyan;
            }
        }

        if (!movement.canMove) return;

        if (Input.GetMouseButtonDown(0))
            StartCoroutine(PerformAttack());

        if (Input.GetMouseButtonDown(1) && isMagicReady)
        {
            StartCoroutine(PerformMagic());
        }
    }

    IEnumerator PerformAttack()
    {
        movement.canMove = false; 
        anim.SetTrigger("Attack"); 

        yield return new WaitForSeconds(0.4f); 
        ApplyPhysicalDamage();

        yield return new WaitForSeconds(attackLockTime - 0.4f);
        movement.canMove = true; 
    }
    IEnumerator PerformMagic()
    {

        isMagicReady = false;
        magicTimer = 0;
        if (magicSlider != null)
        {
            magicSlider.value = 0;
            magicSlider.fillRect.GetComponent<Image>().color = new Color(0, 0.5f, 1f);
        }

        movement.canMove = false;
        anim.SetTrigger("Magic");
        yield return new WaitForSeconds(0.5f);
        Instantiate(magicProjectilePrefab, firePoint.position, transform.rotation);

        yield return new WaitForSeconds(magicLockTime - 0.5f);
        movement.canMove = true;
    }

    void ApplyPhysicalDamage()
    {
        if (swordPoint == null) return;

        Collider[] hitEnemies = Physics.OverlapSphere(swordPoint.position, attackRadius);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.transform == transform) continue; 

            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(physDamage, DamageType.Physical);
                Debug.Log("Меч попал по: " + enemy.name);
                break;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (swordPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(swordPoint.position, attackRadius);
    }
}