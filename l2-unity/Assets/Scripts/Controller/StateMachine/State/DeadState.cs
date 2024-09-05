public class DeadState : StateBase
{
    public DeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void HandleEvent(Event evt)
    {

    }
}