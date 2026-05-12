using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private PlayerModel _model;
    private PlayerView _view;
    private CharacterController _cc;

    [Header("Настройки боя")]
    public GameObject magicPrefab;   // Префаб магического шара
    public Transform firePoint;      // Точка вылета магии
    public Transform swordPoint;     // Точка на мече
    public float attackRadius = 1.5f; // Радиус удара мечом

    private Vector3 _velocity;       // Скорость падения (гравитация)
    public bool canMove = true;      // Флаг блокировки управления

    // МЕТОД ИНИЦИАЛИЗАЦИИ (вызывается из LevelBootstrapper)
    public void Initialize(PlayerModel model, PlayerView view)
    {
        _model = model;
        _view = view;
        _cc = GetComponent<CharacterController>();

        if (_cc == null) Debug.LogError("ОШИБКА: На объекте Player нет компонента CharacterController!");
        if (_view == null) Debug.LogError("ОШИБКА: Ссылка на PlayerView пуста!");

        _model.OnHealthChanged += _view.SetHealth;
        _model.OnMagicTimerChanged += _view.SetMagic;
        _model.OnDeath += HandleDeath;

        _view.Initialize(_model.MaxHealth, _model.MagicCooldown);
        _view.SetHealth(_model.Health);
    }

    // МЕТОДЫ ДЛЯ СИСТЕМЫ СОХРАНЕНИЯ
    public PlayerModel GetModel() => _model;

    public void SetHealthFromSave(float hp)
    {
        _model.Health = hp;
        _view.SetHealth(hp);
    }


    void Update()
    {
        // ПРОВЕРКА 1: Работает ли Update вообще?
        // Раскомментируй строку ниже, чтобы увидеть спам в консоли
        // Debug.Log("Update работает!");

        if (_cc == null || !_cc.enabled) return; // Если контроллер выключен — стоим
        if (Time.timeScale == 0) return;         // Если пауза — стоим

        _model.UpdateMagicTimer(Time.deltaTime);

        if (canMove)
        {
            HandleMovement();
            HandleCombat();
        }
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        float currentSpeed = _model.MoveSpeed;

        // ПРОВЕРКА БЕГА
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && move.magnitude > 0.1f;
        if (isSprinting)
        {
            currentSpeed *= _model.SprintMultiplier;
        }

        // ДВИГАЕМ ПЕРСОНАЖА
        _cc.Move(move * currentSpeed * Time.deltaTime);

        // ГРАВИТАЦИЯ
        if (_cc.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        _velocity.y += -9.81f * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);

        // СВЯЗЬ С АНИМАЦИЕЙ (ИСПРАВЛЕНО)
        // Мы берем "силу нажатия" (x, z) и умножаем на скорость
        float inputMagnitude = new Vector2(x, z).magnitude;
        float speedForAnim = inputMagnitude * (isSprinting ? 5f : 2f); // 2 - ходьба, 5 - бег

        _view.UpdateMoveAnimation(speedForAnim);
    }

    private void HandleCombat()
    {
        // ЛКМ - Физическая атака
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(PhysicalAttackRoutine());
        }

        // ПКМ - Магическая атака (с проверкой кулдауна из модели)
        if (Input.GetMouseButtonDown(1) && _model.IsMagicReady)
        {
            StartCoroutine(MagicAttackRoutine());
        }
    }

    IEnumerator PhysicalAttackRoutine()
    {
        canMove = false;
        _view.PlayAnimation("Attack");

        yield return new WaitForSeconds(0.4f); // Момент взмаха

        // Наносим урон сферой
        Collider[] hits = Physics.OverlapSphere(swordPoint.position, attackRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            // У врагов всё еще старый скрипт Health, так что ищем его
            Health h = hit.GetComponent<Health>();
            if (h != null)
            {
                h.TakeDamage(20, DamageType.Physical);
                break;
            }
        }

        yield return new WaitForSeconds(0.4f); // Конец анимации
        canMove = true;
    }

    IEnumerator MagicAttackRoutine()
    {
        _model.ResetMagicTimer(); // Сбрасываем кулдаун в модели
        canMove = false;
        _view.PlayAnimation("Magic");


        yield return new WaitForSeconds(0.5f);
        Instantiate(magicPrefab, firePoint.position, transform.rotation);

        yield return new WaitForSeconds(0.5f);
        canMove = true;
    }

    // Этот метод вызывается врагами при попадании по игроку
    public void ApplyDamage(float amount)
    {
        if (_model.Health <= 0) return;

        _model.ChangeHealth(-amount);
        _view.PlayAnimation("Hit");
    }

    private void HandleDeath()
    {
        if (!this.enabled) return; // Чтобы не срабатывало дважды

        StartCoroutine(DeathSequenceRoutine());
    }

    IEnumerator DeathSequenceRoutine()
    {
        Debug.Log("Игрок мертв. Запуск последовательности смерти...");

        canMove = false;
        if (_cc != null) _cc.enabled = false; // Отключаем физику, чтобы не упасть сквозь пол

        _view.PlayAnimation("Death"); // Запускаем анимацию падения

        // Ждем 3 секунды, пока рыцарь падает (можешь изменить время под анимацию)
        yield return new WaitForSecondsRealtime(3f);

        // Ищем GameManager на сцене и просим его показать экран смерти
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.ShowGameOverScreen();
        }
        else
        {
            Debug.LogError("КРИТИЧЕСКАЯ ОШИБКА: GameManager не найден на сцене!");
        }

        this.enabled = false; // Выключаем контроллер совсем
    }

    // Рисуем сферу удара в окне Scene
    private void OnDrawGizmosSelected()
    {
        if (swordPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(swordPoint.position, attackRadius);
        }
    }
}
