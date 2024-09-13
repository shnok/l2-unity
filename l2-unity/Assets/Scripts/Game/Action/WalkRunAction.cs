public class WalkRunAction : L2Action
{
    public WalkRunAction() : base() { }

    // Local action
    public override void UseAction()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.DEAD)
        {
            return;
        }

        GameClient.Instance.ClientPacketHandler.RequestActionUse((int)ActionType.WalkRun);
    }
}