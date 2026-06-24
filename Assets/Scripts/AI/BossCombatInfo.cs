using UnityEngine;

public class BossCombatInfo : MonoBehaviour
{
    [Header("Настройки Ближнего Боя")]
    public float weakMeleeDamage = 15f;
    public float strongMeleeDamage = 35f;

    [Header("Настройки Дальнего Боя")]
    public float rangedAttackDistance = 12f;
    public EnemyWeaponData[] magicVariants;

    public float GetMeleeDamage(bool useStrongMelee)
    {
        return useStrongMelee ? strongMeleeDamage : weakMeleeDamage;
    }

    public bool RollStrongMeleeAttack()
    {
        return Random.value > 0.5f;
    }

    public EnemyWeaponData GetRandomMagicVariant()
    {
        if (magicVariants == null || magicVariants.Length == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, magicVariants.Length);
        return magicVariants[randomIndex];
    }
}
