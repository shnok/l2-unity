public class AttackingState : StateBase
{
    public AttackingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (PlayerEntity.Instance.StartAutoAttacking())
        {
            PlayerController.Instance.StartLookAt(TargetManager.Instance.AttackTarget.Data.ObjectTransform);
        }
    }

    public override void Update()
    {
        if (InputManager.Instance.Move || PlayerController.Instance.RunningToDestination && !TargetManager.Instance.HasAttackTarget())
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_MOVE_TO);
        }
    }

    public override void HandleEvent(Event evt)
    {
        if (evt == Event.ACTION_ALLOWED)
        {
            if (_stateMachine.Intention == Intention.INTENTION_MOVE_TO)
            {
                _stateMachine.ChangeState(PlayerState.RUNNING);
            }
        }

        if (evt == Event.ACTION_ALLOWED)
        {
            if (_stateMachine.Intention == Intention.INTENTION_IDLE)
            {
                _stateMachine.ChangeState(PlayerState.IDLE);
            }
        }
    }


    public override void Exit()
    {
        PlayerEntity.Instance.StopAutoAttacking();
        PlayerController.Instance.StopLookAt();
    }
}