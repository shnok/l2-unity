public class SitWaitState : StateBase
{
    public SitWaitState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.ACTION_ALLOWED:
                if (_stateMachine.Intention == Intention.INTENTION_STAND)
                {
                    _stateMachine.ChangeState(PlayerState.STANDING);
                }
                break;
        }
    }
}