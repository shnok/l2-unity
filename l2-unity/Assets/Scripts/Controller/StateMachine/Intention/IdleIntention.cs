public class IdleIntention : IntentionBase
{
    public IdleIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Exit() { }
    public override void Update()
    {
        //Arrived to destination
        if (!InputManager.Instance.Move && !PlayerController.Instance.RunningToDestination)
        {
            _stateMachine.ChangeState(PlayerState.IDLE);
        }

    }
}