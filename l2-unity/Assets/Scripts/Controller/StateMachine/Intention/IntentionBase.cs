
public abstract class IntentionBase
{
    protected PlayerStateMachine _stateMachine;

    public IntentionBase(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void Enter(object arg0) { }
    public virtual void Exit() { }
    public virtual void Update() { }
}