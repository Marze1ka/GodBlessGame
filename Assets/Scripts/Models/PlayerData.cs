using UnityEngine;
using System;

[Serializable] // Позволяет превращать данные в текст (JSON)
public class PlayerData
{
    public float HP;
    public float MP; // Мана (твоя полоска магии)
    public Vector3 Position;
}