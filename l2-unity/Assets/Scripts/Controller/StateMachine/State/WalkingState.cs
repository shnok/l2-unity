public class WalkingState : StateBase
{
    public WalkingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object obj0)
    {
        NewPlayerAnimationController.Instance.Walk();
    }

    public override void Update()
    {
        //Arrived to destination
        if (!InputManager.Instance.Move && !PlayerController.Instance.RunningToDestination)
        {
            _stateMachine.NotifyEvent(Event.ARRIVED);
        }

        // If move input is pressed while running to target
        if (TargetManager.Instance.HasAttackTarget() && InputManager.Instance.Move)
        {
            // Cancel follow target
            TargetManager.Instance.ClearAttackTarget();
        }
    }


    public override void Exit()
    {
        base.Exit();
        PlayerController.Instance.IntentionToRun = false;
    }

    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.ARRIVED:
                if (TargetManager.Instance.HasAttackTarget())
                {
                    // _stateMachine.ChangeIntention(Intention.INTENTION_ATTACK, AttackIntentionType.TargetReached);
                    _stateMachine.ChangeIntention(Intention.INTENTION_ATTACK);
                }
                else
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                }
                break;
            case Event.ACTION_ALLOWED:
                if (_stateMachine.Intention == Intention.INTENTION_SIT)
                {
                    _stateMachine.ChangeState(PlayerState.SITTING);
                }
                break;
            case Event.MOVE_TYPE_UPDATED:
                if (PlayerEntity.Instance.Running)
                {
                    _stateMachine.ChangeState(PlayerState.RUNNING);
                }
                break;
            case Event.DEAD:
                _stateMachine.ChangeState(PlayerState.DEAD);
                break;
        }
    }
}