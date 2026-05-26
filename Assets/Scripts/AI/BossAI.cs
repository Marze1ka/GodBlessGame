using UnityEngine;

public class BossAI : EnemyBaseAI
{
    [Header("Настройки Ближнего Боя")]
    public float weakAttackDamage = 15f;
    public float strongAttackDamage = 35f;
    [HideInInspector] public bool isUsingStrongMelee; // Выберется случайно

    [Header("Настройки Дальнего Боя (Твои префабы)")]
    public GameObject[] bossProjectiles; // Сюда перетащи 4 своих префаба
    public float[] projectileDamages;    // Урон для каждого из 4-х снарядов
    public float rangedDistance = 12f;   // Дистанция стрельбы

    [HideInInspector] public float currentMagicDamage; // Выбранный урон магии

    // Добавили override, чтобы исправить ошибку CS0114
    protected override void Start()
    {
        // Сначала выполняем базовые настройки из EnemyBaseAI (поиск игрока и т.д.)
        base.Start();

        // Затем настраиваем уникальность Босса
        isUsingStrongMelee = Random.value > 0.5f;

        if (bossProjectiles.Length >= 4)
        {
            int randomIndex = Random.Range(0, bossProjectiles.Length);
            projectilePrefab = bossProjectiles[randomIndex];
            currentMagicDamage = projectileDamages[randomIndex];
        }

        // Перезаписываем начальное состояние на Покой Босса
        stateMachine.Initialize(new BossIdleState(stateMachine, this));
    }

    private void Update()
    {
        stateMachine.Update();
        // Логика 2-й фазы (ускорение) остается автоматически
    }

    public override void OnHit()
    {
        if (stateMachine.CurrentState is BossIdleState)
        {
            stateMachine.ChangeState(new BossChaseState(stateMachine, this));
        }
    }
}