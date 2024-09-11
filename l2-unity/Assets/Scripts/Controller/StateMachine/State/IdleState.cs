
using UnityEngine;

public class IdleState : StateBase
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }


    public override void Update()
    {
        // Does the player want to move ?
        if (InputManager.Instance.Move || PlayerController.Instance.RunningToDestination && !TargetManager.Instance.HasAttackTarget())
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_MOVE_TO);
        }
        else if (PlayerController.Instance.RunningToDestination && TargetManager.Instance.HasAttackTarget())
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_FOLLOW);
        }
    }

    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.READY_TO_ACT:
                if (TargetManager.Instance.HasAttackTarget() && !_stateMachine.WaitingForServerReply)
                {
                    //Debug.Log("On Reaching Target");
                    PathFinderController.Instance.ClearPath();
                    PlayerController.Instance.ResetDestination();

                    if (_stateMachine.NetworkTransformShare != null)
                    {
                        _stateMachine.NetworkTransformShare.SharePosition();
                    }

                    NetworkCharacterControllerShare.Instance.ForceShareMoveDirection();

                    if (TargetManager.Instance.IsAttackTargetSet())
                    {
                        GameClient.Instance.ClientPacketHandler.SendRequestAutoAttack(-1);
                    }
                    else
                    {
                        Debug.LogWarning("[StateMachine] Attacking a target which is not current target.");
                        GameClient.Instance.ClientPacketHandler.SendRequestAutoAttack(TargetManager.Instance.AttackTarget.Identity.Id);
                    }

                    _stateMachine.SetWaitingForServerReply(true);
                }
                else
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                }
                break;
            case Event.ACTION_ALLOWED:
                if (_stateMachine.Intention == Intention.INTENTION_ATTACK)
                {
                    _stateMachine.ChangeState(PlayerState.ATTACKING);
                }
                if (_stateMachine.Intention == Intention.INTENTION_SIT)
                {
                    _stateMachine.ChangeState(PlayerState.SITTING);
                }
                break;
            case Event.ACTION_DENIED:
                break;

        }
    }
}