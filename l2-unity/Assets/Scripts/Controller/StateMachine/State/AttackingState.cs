public class AttackingState : StateBase
{
    public AttackingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (PlayerCombat.Instance.StartAutoAttacking())
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
        switch (evt)
        {
            case Event.ACTION_ALLOWED:
                if (_stateMachine.Intention == Intention.INTENTION_MOVE_TO)
                {
                    if (PlayerEntity.Instance.Running)
                    {
                        _stateMachine.ChangeState(PlayerState.RUNNING);
                    }
                    else
                    {
                        _stateMachine.ChangeState(PlayerState.WALKING);
                    }
                }
                if (_stateMachine.Intention == Intention.INTENTION_IDLE)
                {
                    _stateMachine.ChangeState(PlayerState.IDLE);
                }
                if (_stateMachine.Intention == Intention.INTENTION_SIT)
                {
                    _stateMachine.ChangeState(PlayerState.SITTING);
                }
                break;
            // Auto attack stop event
            case Event.CANCEL:
                _stateMachine.ChangeState(PlayerState.IDLE);

                if (_stateMachine.Intention == Intention.INTENTION_FOLLOW)
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_ATTACK, AttackIntentionType.ChangeTarget);
                }
                break;
        }
    }

    public enum AttackIntentionType
    {
        ChangeTarget,
        AttackInput,
        TargetReached
    }


    public override void Exit()
    {
        PlayerCombat.Instance.StopAutoAttacking();
        PlayerController.Instance.StopLookAt();
    }
}