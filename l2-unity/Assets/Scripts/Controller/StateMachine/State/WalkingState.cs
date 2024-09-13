using static AttackingState;

public class WalkingState : StateBase
{
    public WalkingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.ARRIVED:
                if (TargetManager.Instance.HasAttackTarget())
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_ATTACK, AttackIntentionType.TargetReached);
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
        }
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
}