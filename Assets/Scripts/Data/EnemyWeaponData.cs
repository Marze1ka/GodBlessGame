using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "RPG/Enemy Weapon Data")]
public class EnemyWeaponData : ScriptableObject
{
    public string variantName;       // Имя (например, "Мечник" или "Огненный маг")
    public GameObject weaponPrefab;  // Модель меча (для ближника)
    public GameObject projectilePrefab; // Префаб снаряда (для дальника)
    public float damage = 10f;       // Урон этого варианта
}