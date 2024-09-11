using UnityEngine;

public class SitStandAction : L2Action
{
    public SitStandAction() : base() { }

    // Local action
    public override void UseAction()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.DEAD)
        {
            return;
        }

        if (PlayerStateMachine.Instance.State == PlayerState.SITTING)
        {
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_IDLE);
        }
        else
        {
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_SIT);
        }
    }
}