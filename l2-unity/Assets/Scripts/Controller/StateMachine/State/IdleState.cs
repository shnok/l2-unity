
using UnityEngine;

public class IdleState : StateBase
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object obj0)
    {
        NewPlayerAnimationController.Instance.Wait();
    }

    public override void Update()
    {
        if (InputManager.Instance.Move)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_MOVE);
        }
    }

    public override void HandleEvent(Event evt, object arg0)
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

                    GameClient.Instance.ClientPacketHandler.RequestAttackForce(TargetManager.Instance.Target.Identity.Id);

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
            case Event.READY_TO_SKILL:
                if (arg0 == null)
                {
                    Debug.LogWarning("READY_TO_SKILL event does not have a skill attached to.");
                }
                Skill skill = (Skill)arg0;
                GameClient.Instance?.ClientPacketHandler.RequestMagicSkillUse(skill.SkillId, true, false);
                break;
            case Event.ACTION_DENIED:
                break;
            case Event.DEAD:
                _stateMachine.ChangeState(PlayerState.DEAD);
                break;

        }
    }
}