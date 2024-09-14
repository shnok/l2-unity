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

        if (PlayerStateMachine.Instance.State == PlayerState.SITTING || PlayerStateMachine.Instance.State == PlayerState.SIT_WAIT)
        {
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_STAND);
        }
        else
        {
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_SIT);
        }
    }
}