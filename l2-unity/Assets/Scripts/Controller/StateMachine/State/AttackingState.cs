using UnityEngine.InputSystem;

public class AttackingState : StateBase
{
    public AttackingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object obj0)
    {
        // PlayerCombat.Instance.StartAttackStance();
        // PlayerController.Instance.StartLookAt(TargetManager.Instance.AttackTarget.Data.ObjectTransform);
    }

    public override void Update()
    {
        if (InputManager.Instance.Move)
        // if (InputManager.Instance.Move || PlayerController.Instance.RunningToDestination && !TargetManager.Instance.HasAttackTarget())
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_MOVE);
        }
        // else if (!TargetManager.Instance.HasAttackTarget() || TargetManager.Instance.HasAttackTarget() && TargetManager.Instance.AttackTarget.Status.IsDead)
        else if (TargetManager.Instance.HasAttackTarget() && TargetManager.Instance.AttackTarget.Status.IsDead)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
        }
    }

    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.ACTION_ALLOWED:
                NetworkCharacterControllerShare.Instance.ForceShareMoveDirection();
                if (_stateMachine.Intention == Intention.INTENTION_MOVE)
                {
                    if (!InputManager.Instance.Move)
                    {
                        _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                        return;
                    }

                    if (PlayerEntity.Instance.Running)
                    {
                        _stateMachine.ChangeState(PlayerState.RUNNING);
                    }
                    else
                    {
                        _stateMachine.ChangeState(PlayerState.WALKING);
                    }
                }
                if (_stateMachine.Intention == Intention.INTENTION_MOVE_TO)
                {
                    if (!PlayerController.Instance.IntentionToRun)
                    {
                        _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                        return;
                    }

                    //Set state as running first to change to movable state
                    if (PlayerEntity.Instance.Running)
                    {
                        _stateMachine.ChangeState(PlayerState.RUNNING);
                    }
                    else
                    {
                        _stateMachine.ChangeState(PlayerState.WALKING);
                    }

                    _stateMachine.ChangeIntention(Intention.INTENTION_MOVE_TO); //not giving an argument will use last position as destination
                }
                if (_stateMachine.Intention == Intention.INTENTION_FOLLOW)
                {
                    if (PlayerEntity.Instance.Running)
                    {
                        _stateMachine.ChangeState(PlayerState.RUNNING, FollowIntention.MoveReason);
                    }
                    else
                    {
                        _stateMachine.ChangeState(PlayerState.WALKING, FollowIntention.MoveReason);
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
                    // _stateMachine.ChangeIntention(Intention.INTENTION_ATTACK, AttackIntentionType.ChangeTarget);
                    _stateMachine.ChangeIntention(Intention.INTENTION_ATTACK);
                }
                break;
            case Event.DEAD:
                _stateMachine.ChangeState(PlayerState.DEAD);
                break;
        }
    }

    public override void Exit()
    {
        // PlayerCombat.Instance.StopAttackStance();
        PlayerController.Instance.StopLookAt();
    }
}