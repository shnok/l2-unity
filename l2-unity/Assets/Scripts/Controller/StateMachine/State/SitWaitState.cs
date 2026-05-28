public class SitWaitState : StateBase
{
    public SitWaitState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object obj0)
    {
        NewPlayerAnimationController.Instance.SitWait();
    }

    public override void HandleEvent(Event evt, object arg0)
    {
        switch (evt)
        {
            case Event.ACTION_ALLOWED:
                if (_stateMachine.Intention == Intention.INTENTION_STAND)
                {
                    _stateMachine.ChangeState(PlayerState.STANDING);
                }
                break;
            case Event.DEAD:
                _stateMachine.ChangeState(PlayerState.DEAD);
                break;
        }
    }
}