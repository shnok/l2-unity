public class StandIntention : IntentionBase
{
    public StandIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        if (!_stateMachine.WaitingForServerReply)
        {
            _stateMachine.SetWaitingForServerReply(true);
            GameClient.Instance.ClientPacketHandler.RequestActionUse((int)ActionType.Sit);
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