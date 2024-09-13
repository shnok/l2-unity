public class NextTargetAction : L2Action
{
    public NextTargetAction() : base() { }

    // Local action
    public override void UseAction()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.DEAD)
        {
            return;
        }

        TargetManager.Instance.NextTarget();
    }
}