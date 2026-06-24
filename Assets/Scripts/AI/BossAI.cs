using UnityEngine;

public class BossAI : EnemyBaseAI
{
    [Header("Боевые данные босса")]
    public BossCombatInfo combatInfo;

    public bool UseStrongMelee { get; private set; }
    public float RangedDistance => combatInfo != null ? combatInfo.rangedAttackDistance : rangedStopDistance;

    private EnemyWeaponData currentMagicVariant;

    protected override void Awake()
    {
        base.Awake();

        if (combatInfo == null)
        {
            combatInfo = GetComponent<BossCombatInfo>();
        }
    }

    protected override void Start()
    {
        SelectMagicVariant();
        base.Start();
    }

    public void SelectMeleeAttackType()
    {
        if (combatInfo == null)
        {
            UseStrongMelee = false;
            return;
        }

        UseStrongMelee = combatInfo.RollStrongMeleeAttack();
    }

    public void SelectMagicVariant()
    {
        currentMagicVariant = combatInfo != null ? combatInfo.GetRandomMagicVariant() : null;
        projectilePrefab = currentMagicVariant != null ? currentMagicVariant.projectilePrefab : null;
    }

    protected override EnemyState CreateInitialState()
    {
        return new BossIdleState(stateMachine, this);
    }

    protected override void EnterCombatState()
    {
        stateMachine.ChangeState(new BossChaseState(stateMachine, this));
    }

    public override void OnHit()
    {
        if (stateMachine.CurrentState == null)
        {
            return;
        }

        if (stateMachine.CurrentState.GetType() == typeof(BossIdleState))
        {
            EnterCombatState();
        }
    }

    protected override float GetMeleeDamage()
    {
        if (combatInfo == null)
        {
            return base.GetMeleeDamage();
        }

        return combatInfo.GetMeleeDamage(UseStrongMelee);
    }

    protected override void ExecuteMeleeLogic()
    {
        if (!ResolvePlayerReference())
        {
            return;
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            playerController = Object.FindAnyObjectByType<PlayerController>();
        }

        if (playerController == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);
        if (distanceToPlayer > attackRange + 1.5f)
        {
            return;
        }

        playerController.ApplyDamage(GetMeleeDamage());
        Debug.Log("<color=red>BOSS MELEE HIT</color>");
    }

    protected override void ConfigureProjectile(GameObject projectileObject)
    {
        if (currentMagicVariant == null)
        {
            return;
        }

        EnemyProjectile projectile = projectileObject.GetComponent<EnemyProjectile>();
        if (projectile != null)
        {
            projectile.damage = currentMagicVariant.damage;
        }
    }
}
