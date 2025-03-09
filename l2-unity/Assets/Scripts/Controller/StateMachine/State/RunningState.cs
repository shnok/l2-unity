using UnityEngine;
using static AttackingState;

public class RunningState : StateBase
{
    private MoveReason _moveReason = MoveReason.DEFAULT;

    public RunningState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.ARRIVED:
                if (_moveReason == MoveReason.ATTACK)
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_ATTACK);
                }
                else if (_moveReason == MoveReason.INTERACT)
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_INTERACT);
                }
                else if (_moveReason == MoveReason.SKILL)
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_SKILL);
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
                if (!PlayerEntity.Instance.Running)
                {
                    _stateMachine.ChangeState(PlayerState.WALKING);
                }
                break;
            case Event.DEAD:
                _stateMachine.ChangeState(PlayerState.DEAD);
                break;
        }
    }

    public override void Enter(object arg0)
    {
        NewPlayerAnimationController.Instance.Run();

        if (arg0 == null || arg0 is Vector3)
        {
            _moveReason = MoveReason.DEFAULT;
        }
        else
        {
            _moveReason = (MoveReason)arg0;
        }
    }

    public override void Exit()
    {
        base.Exit();
        PlayerController.Instance.IntentionToRun = false;
    }

    public override void Update()
    {
        if (InputManager.Instance.Move)
        {
            _moveReason = MoveReason.DEFAULT;
        }

        //Arrived to destination
        if (!InputManager.Instance.Move && !PlayerController.Instance.RunningToDestination && !PlayerController.Instance.IntentionToRun)
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