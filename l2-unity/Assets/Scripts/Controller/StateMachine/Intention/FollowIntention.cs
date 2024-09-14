using UnityEngine;

public class FollowIntention : IntentionBase
{
    public FollowIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        if (arg0 != null)
        {
            PathFinderController.Instance.MoveTo((Vector3)arg0);
        }

        if (_stateMachine.IsInMovableState())
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
        else if (!_stateMachine.WaitingForServerReply)
        {
            _stateMachine.SetWaitingForServerReply(true);
            GameClient.Instance.ClientPacketHandler.UpdateMoveDirection(Vector3.zero);
        }
        else
        {
            PlayerController.Instance.StopMoving();
        }
    }

    public override void Exit() { }
    public override void Update() { }
}