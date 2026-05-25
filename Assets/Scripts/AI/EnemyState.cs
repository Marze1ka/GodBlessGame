using UnityEngine;

public abstract class EnemyState
{
    protected EnemyStateMachine stateMachine;
    protected EnemyBaseAI context; // Ссылка на самого моба и его данные

    public EnemyState(EnemyStateMachine machine, EnemyBaseAI context)
    {
        this.stateMachine = machine;
        this.context = context;
    }

    public virtual void Enter() { }   // Выполняется один раз при входе в состояние
    public virtual void Update() { }  // Выполняется каждый кадр
    public virtual void Exit() { }    // Выполняется при выходе
}