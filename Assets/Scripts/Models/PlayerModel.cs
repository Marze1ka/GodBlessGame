using System;
using UnityEngine;

public class PlayerModel
{
    // Данные здоровья
    public float Health;
    public float MaxHealth = 100f;

    // Данные магии (кулдаун)
    public float MagicCooldown = 3f;
    public float MagicTimer;
    public bool IsMagicReady => MagicTimer >= MagicCooldown;

    // Настройки движения
    public float MoveSpeed = 5f;
    public float SprintMultiplier = 2f;

    // События (чтобы контроллер знал, когда обновить визуал)
    public Action<float> OnHealthChanged;
    public Action<float> OnMagicTimerChanged;
    public Action OnDeath;

    public PlayerModel()
    {
        Health = MaxHealth;
        MagicTimer = MagicCooldown;
    }

    public void ChangeHealth(float amount)
    {
        Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
        OnHealthChanged?.Invoke(Health);
        if (Health <= 0) OnDeath?.Invoke();
    }

    public void UpdateMagicTimer(float deltaTime)
    {
        if (MagicTimer < MagicCooldown)
        {
            MagicTimer += deltaTime;
            OnMagicTimerChanged?.Invoke(MagicTimer);
        }
    }

    public void ResetMagicTimer()
    {
        MagicTimer = 0;
        OnMagicTimerChanged?.Invoke(0);
    }
}