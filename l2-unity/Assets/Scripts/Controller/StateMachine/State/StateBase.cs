public abstract class StateBase
{
    protected PlayerStateMachine _stateMachine;

    public StateBase(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void HandleEvent(Event evt) { }
}