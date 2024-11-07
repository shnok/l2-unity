using UnityEngine;

public class MoveIntention : IntentionBase
{
    public MoveIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        if (_stateMachine.State == PlayerState.SITTING || _stateMachine.State == PlayerState.SIT_WAIT || _stateMachine.State == PlayerState.STANDING)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_STAND);
            return;
        }

        if (_stateMachine.IsInMovableState())
        {
            RunOrWalk(MoveReason.DEFAULT);
        }
        else if (!_stateMachine.WaitingForServerReply)
        {
            _stateMachine.SetWaitingForServerReply(true);
            NetworkCharacterControllerShare.Instance.ForceShareMoveDirection();
        }
        else
        {
            PlayerController.Instance.StopMoving();
        }
    }

    private void RunOrWalk(MoveReason moveReason)
    {
        if (PlayerEntity.Instance.Running)
        {
            _stateMachine.ChangeState(PlayerState.RUNNING, moveReason);
        }
        else
        {
            _stateMachine.ChangeState(PlayerState.WALKING, moveReason);
        }
    }

    public override void Exit() { }
    public override void Update()
    {
        if (_stateMachine.WaitingForServerReply)
        {
            if (InputManager.Instance.Move)
            {
                NetworkCharacterControllerShare.Instance.ForceShareMoveDirection();
            }
            PlayerController.Instance.StopMoving();
        }
    }
}