
using UnityEngine;

public class IdleState : StateBase
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }


    public override void Update()
    {
        // Does the player want to move ?
        // if (InputManager.Instance.Move || PlayerController.Instance != null && PlayerController.Instance.RunningToDestination && !TargetManager.Instance.HasAttackTarget())
        // {
        //     _stateMachine.ChangeIntention(Intention.INTENTION_MOVE_TO);
        // }
        // else if (PlayerController.Instance != null && PlayerController.Instance.RunningToDestination && TargetManager.Instance.HasAttackTarget())
        // {
        //     _stateMachine.ChangeIntention(Intention.INTENTION_FOLLOW);
        // }

        if (InputManager.Instance.Move)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_MOVE);
        }
    }

    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.READY_TO_INTERACT:
                PathFinderController.Instance.ClearPath();
                PlayerController.Instance.ResetDestination(false);
                NetworkTransformShare.Instance.SharePosition();
                NetworkCharacterControllerShare.Instance.ShareMoveDirection(Vector3.zero);

                // Wait for server reply?
                GameClient.Instance.ClientPacketHandler.SendRequestAction(TargetManager.Instance.Target.Identity.Id);
                break;
            case Event.READY_TO_ATTACK:
                if (!_stateMachine.WaitingForServerReply)
                {
                    PathFinderController.Instance.ClearPath();
                    PlayerController.Instance.ResetDestination(false);
                    NetworkTransformShare.Instance.SharePosition();
                    NetworkCharacterControllerShare.Instance.ShareMoveDirection(Vector3.zero);

                    if (TargetManager.Instance.IsAttackTargetSet())
                    {
                        GameClient.Instance.ClientPacketHandler.RequestAttackForce(TargetManager.Instance.AttackTarget.Identity.Id);
                    }

                    _stateMachine.SetWaitingForServerReply(true);
                }
                else
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                }
                break;
            case Event.ATTACK_ALLOWED:
                if (_stateMachine.Intention == Intention.INTENTION_ATTACK)
                {
                    _stateMachine.ChangeState(PlayerState.ATTACKING);
                }
                break;
            case Event.ACTION_ALLOWED:
                if (_stateMachine.Intention == Intention.INTENTION_SIT)
                {
                    _stateMachine.ChangeState(PlayerState.SITTING);
                }
                break;
            case Event.ACTION_DENIED:
                break;
            case Event.DEAD:
                _stateMachine.ChangeState(PlayerState.DEAD);
                break;

        }
    }
}