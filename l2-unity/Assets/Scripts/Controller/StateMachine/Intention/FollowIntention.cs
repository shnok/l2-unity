using UnityEngine;

public class FollowIntention : IntentionBase
{
    public static MoveReason MoveReason { get; private set; }

    public FollowIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        if (_stateMachine.State == PlayerState.SITTING || _stateMachine.State == PlayerState.SIT_WAIT || _stateMachine.State == PlayerState.STANDING)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_STAND);
            return;
        }

        MoveReason = (MoveReason)arg0;

        PlayerController.Instance.IntentionToRun = true;

        if (_stateMachine.IsInMovableState())
        {
            RunOrWalk(MoveReason);
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

    public void RunOrWalk(MoveReason moveReason)
    {
        _stateMachine.ChangeState(PlayerState.MOVING, moveReason);
    }

    public override void Exit() { }
    public override void Update()
    {
        // if (_stateMachine.WaitingForServerReply)
        // {
        //     if (InputManager.Instance.Move)
        //     {
        //         NetworkCharacterControllerShare.Instance.ForceShareMoveDirection();
        //     }
        //     PlayerController.Instance.StopMoving();
        // }
    }
}