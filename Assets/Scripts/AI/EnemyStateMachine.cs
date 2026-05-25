using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    // Добавили public property, чтобы другие скрипты (как BossAI) могли видеть состояние
    public EnemyState CurrentState { get; private set; }

    public void Initialize(EnemyState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    // Сделали public, чтобы можно было вызывать вручную, если нужно
    public void Update()
    {
        // Если объект выключен (например, при смерти), не обновляем машину состояний
        if (!this.enabled) return;

        CurrentState?.Update();
    }
}