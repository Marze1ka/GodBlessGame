using UnityEngine;
using UnityEngine.UI; // Обязательно для работы со слайдером
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ссылки на компоненты")]
    public PlayerMovement movement;    // Ссылка на скрипт движения
    public Animator anim;              // Ссылка на аниматор

    [Header("Настройки урона")]
    public float physDamage = 20f;     // Физический урон
    public float magicDamage = 15f;    // Магический урон
    public float attackRadius = 1.5f;  // Радиус "пузыря" удара мечом

    [Header("Точки атаки (Сюда тянуть объекты)")]
    public Transform swordPoint;       // ТОЧКА МЕЧА (Та самая переменная!)
    public Transform firePoint;        // Точка вылета магии
    public GameObject magicProjectilePrefab; // Префаб магического шара

    [Header("Магия и Кулдаун")]
    public Slider magicSlider;         // Голубой слайдер отката
    public float magicCooldown = 3f;   // Время перезарядки (в сек)
    private float magicTimer;          // Текущее время таймера
    private bool isMagicReady = true;  // Готова ли магия сейчас?

    [Header("Тайминги анимаций (Lock)")]
    public float attackLockTime = 0.8f; // Сколько нельзя ходить при ударе
    public float magicLockTime = 1.2f;  // Сколько нельзя ходить при магии

    void Start()
    {
        // В начале игры магия готова
        magicTimer = magicCooldown;

        if (magicSlider != null)
        {
            magicSlider.maxValue = magicCooldown;
            magicSlider.value = magicCooldown;
            // Устанавливаем голубой цвет при старте
            magicSlider.fillRect.GetComponent<Image>().color = Color.cyan;
        }
    }

    void Update()
    {
        // 1. ЛОГИКА ВОССТАНОВЛЕНИЯ КУЛДАУНА
        if (!isMagicReady)
        {
            magicTimer += Time.deltaTime; // Увеличиваем таймер

            if (magicSlider != null)
                magicSlider.value = magicTimer; // Двигаем полоску вправо

            if (magicTimer >= magicCooldown)
            {
                isMagicReady = true;
                magicTimer = magicCooldown;
                // Меняем цвет на ярко-голубой, когда готово
                if (magicSlider != null)
                    magicSlider.fillRect.GetComponent<Image>().color = Color.cyan;
            }
        }

        // 2. ПРОВЕРКА ВВОДА (Если игрок не в "замке" анимации)
        if (!movement.canMove) return;

        // ЛКМ - Удар мечом
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(PerformAttack());

        // ПКМ - Магия (С проверкой готовности)
        if (Input.GetMouseButtonDown(1) && isMagicReady)
        {
            StartCoroutine(PerformMagic());
        }
    }

    // КОРУТИНА УДАРА МЕЧОМ
    IEnumerator PerformAttack()
    {
        movement.canMove = false; // "Замораживаем" игрока
        anim.SetTrigger("Attack"); // Запуск анимации

        yield return new WaitForSeconds(0.4f); // Ждем момента взмаха
        ApplyPhysicalDamage();

        yield return new WaitForSeconds(attackLockTime - 0.4f);
        movement.canMove = true; // Снова можно ходить
    }

    // КОРУТИНА МАГИИ
    IEnumerator PerformMagic()
    {
        // СБРОС КУЛДАУНА
        isMagicReady = false;
        magicTimer = 0;
        if (magicSlider != null)
        {
            magicSlider.value = 0;
            // Делаем полоску темнее, пока она заряжается
            magicSlider.fillRect.GetComponent<Image>().color = new Color(0, 0.5f, 1f);
        }

        movement.canMove = false;
        anim.SetTrigger("Magic");

        yield return new WaitForSeconds(0.5f); // Ждем замаха рукой
        Instantiate(magicProjectilePrefab, firePoint.position, transform.rotation);

        yield return new WaitForSeconds(magicLockTime - 0.5f);
        movement.canMove = true;
    }

    // ЛОГИКА НАНЕСЕНИЯ УРОНА МЕЧОМ (СФЕРА)
    void ApplyPhysicalDamage()
    {
        if (swordPoint == null) return;

        // Создаем невидимую сферу в точке меча
        Collider[] hitEnemies = Physics.OverlapSphere(swordPoint.position, attackRadius);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.transform == transform) continue; // Не бьем себя

            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(physDamage, DamageType.Physical);
                Debug.Log("Меч попал по: " + enemy.name);
                break; // Ударяем только одного врага за раз
            }
        }
    }

    // РИСУЕМ ОБЛАСТЬ УДАРА В ОКНЕ SCENE (Для настройки)
    private void OnDrawGizmosSelected()
    {
        if (swordPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(swordPoint.position, attackRadius);
    }
}