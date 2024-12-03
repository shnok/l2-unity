public abstract class GameStateBase
{
    protected GameManager _stateMachine;

    public GameStateBase(GameManager stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void Enter(object arg0) { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void HandleEvent(GameEvent evt) { }
}