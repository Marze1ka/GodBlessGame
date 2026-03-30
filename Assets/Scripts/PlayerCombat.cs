using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ссылки")]
    public PlayerMovement movement;    // Ссылка на скрипт движения
    public Animator anim;              // Ссылка на аниматор

    [Header("Настройки урона")]
    public float physDamage = 20f;     // Физический урон
    public float magicDamage = 15f;    // Магический урон
    public float attackRange = 3f;     // Дальность удара мечом

    [Header("Точки атаки")]
    public Transform swordPoint;       // Ссылка на меч в руке (НОВОЕ!)
    public Transform firePoint;        // Точка вылета магии
    public GameObject magicProjectilePrefab; // Префаб магического шара
    [Header("Тайминги анимаций (в секундах)")]
    public float attackLockTime = 1.0f; // Сколько стоим на месте при ударе
    public float magicLockTime = 1.2f;  // Сколько стоим на месте при магии

    void Update()
    {
        // Если ходить нельзя — атаковать тоже нельзя (ждем конца анимации)
        if (!movement.canMove) return;

        // ЛКМ - Удар мечом
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(PerformAttack());

        // ПКМ - Магическая атака
        if (Input.GetMouseButtonDown(1))
            StartCoroutine(PerformMagic());
    }

    IEnumerator PerformAttack()
    {
        movement.canMove = false; // "Замораживаем" игрока
        anim.SetTrigger("Attack"); // Запуск анимации взмаха

        // Ждем момента, когда меч в анимации окажется впереди (подбери под свою анимацию)
        yield return new WaitForSeconds(0.4f);

        ApplyPhysicalDamage(); // Наносим урон

        // Ждем окончания анимации, прежде чем дать игроку снова ходить
        yield return new WaitForSeconds(attackLockTime - 0.4f);
        movement.canMove = true;
    }

    void ApplyPhysicalDamage()
    {
        // ПУСКАЕМ ЛУЧ ОТ МЕЧА
        // Это позволит попадать по врагам именно мечом
        RaycastHit hit;

        // Выпускаем невидимый луч из точки меча вперед (синяя стрелка Z в Unity)
        // Мы используем swordPoint.forward, чтобы луч летел туда, куда направлен меч
        if (Physics.Raycast(swordPoint.position, swordPoint.forward, out hit, attackRange))
        {
            Health enemyHealth = hit.transform.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(physDamage, DamageType.Physical);
                Debug.Log("Попал мечом по: " + hit.transform.name);
            }
        }

        // ВИЗУАЛИЗАЦИЯ: Чтобы ты видел луч в окне Scene (красная линия на 1 сек)
        Debug.DrawRay(swordPoint.position, swordPoint.forward * attackRange, Color.red, 1f);
    }

    IEnumerator PerformMagic()
    {
        movement.canMove = false;
        anim.SetTrigger("Magic");

        yield return new WaitForSeconds(0.5f); // Ждем замаха рукой для магии
        Instantiate(magicProjectilePrefab, firePoint.position, transform.rotation);

        yield return new WaitForSeconds(magicLockTime - 0.5f);
        movement.canMove = true;
    }
}