using UnityEngine;

public class IdleIntention : IntentionBase
{
    public IdleIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        if (_stateMachine.State == PlayerState.RUNNING || _stateMachine.State == PlayerState.IDLE)
        {
            _stateMachine.ChangeState(PlayerState.IDLE);
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

    }
}