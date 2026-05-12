using UnityEngine;
using System;

[Serializable] // Обязательно для сохранения в JSON
public class EnemyData
{
    public string enemyID;   // Уникальное имя объекта, чтобы знать, кого куда ставить
    public float currentHP;
    public Vector3 position;
}