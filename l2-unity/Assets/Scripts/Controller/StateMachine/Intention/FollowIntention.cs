using UnityEngine;

public class FollowIntention : IntentionBase
{
    public FollowIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        _stateMachine.ChangeState(PlayerState.RUNNING);

        if (arg0 != null)
        {
            PathFinderController.Instance.MoveTo((Vector3)arg0);
        }


        // if (CanMove())
        // {
        //     SetState(PlayerState.RUNNING);
        // }
        // else if (!_waitingForServerReply)
        // {
        //     SetWaitingForServerReply(true);
        //     GameClient.Instance.ClientPacketHandler.UpdateMoveDirection(Vector3.zero);
        // }
        // else
        // {
        //     PlayerController.Instance.StopMoving();
        // }
    }
    public override void Exit() { }
    public override void Update() { }
}