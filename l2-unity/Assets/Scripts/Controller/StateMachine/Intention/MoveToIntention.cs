using UnityEngine;

public class MoveToIntention : IntentionBase
{
    public MoveToIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        // _stateMachine.ChangeState(PlayerState.RUNNING);

        if (arg0 != null)
        {
            PathFinderController.Instance.MoveTo((Vector3)arg0);
        }

        if (_stateMachine.State == PlayerState.RUNNING || _stateMachine.State == PlayerState.IDLE)
        {
            _stateMachine.ChangeState(PlayerState.RUNNING);
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
    public override void Update()
    {
        if (_stateMachine.WaitingForServerReply)
        {
            PlayerController.Instance.StopMoving();
        }
    }
}