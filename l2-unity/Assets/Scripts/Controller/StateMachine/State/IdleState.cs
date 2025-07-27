
using UnityEngine;

public class IdleState : StateBase
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object wasCanceled)
    {
        if (NewPlayerAnimationController.Instance.LastAnimationType == HumanoidWeaponAnimType.cast_throw
        || NewPlayerAnimationController.Instance.LastAnimationType == HumanoidWeaponAnimType.cast
        || NewPlayerAnimationController.Instance.LastAnimationType == HumanoidWeaponAnimType.atk)
        {
            if (wasCanceled == null || (bool)wasCanceled == false)
            {
                // Wait for cast throw to finish -> wait is called at the end of the animation anyway
                return;
            }

        }

        NewPlayerAnimationController.Instance.Wait();
    }

    public override void Update()
    {
        if (InputManager.Instance.Jump)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_JUMP);
            return;
        }
        if (InputManager.Instance.Move)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_MOVE);
        }
        if (InputManager.Instance.Jump)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_JUMP);
        }
    }

    public override void HandleEvent(Event evt, object arg0)
    {
        switch (evt)
        {
            case Event.CLICK_TO_MOVE:
                _stateMachine.ChangeIntention(Intention.INTENTION_MOVE_TO, (Vector3)arg0);
                break;
            case Event.READY_TO_INTERACT:
                PathFinderController.Instance.ClearPath();
                PlayerController.Instance.ResetDestination(false);
                NetworkTransformShare.Instance.SharePosition();
                NetworkCharacterControllerShare.Instance.ShareMoveDirection(Vector3.zero, 0.0f, Vector3.zero);

                // Wait for server reply?
                GameClient.Instance.ClientPacketHandler.SendRequestAction(TargetManager.Instance.Target.Identity.Id);
                break;
            case Event.READY_TO_ATTACK:
                if (!_stateMachine.WaitingForServerReply)
                {
                    PathFinderController.Instance.ClearPath();
                    PlayerController.Instance.ResetDestination(false);
                    NetworkTransformShare.Instance.SharePosition();
                    NetworkCharacterControllerShare.Instance.ShareMoveDirection(Vector3.zero, 0.0f, Vector3.zero);

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
                SkillInfo skill = (SkillInfo)arg0;
                PathFinderController.Instance.ClearPath();
                PlayerController.Instance.ResetDestination(false);
                NetworkTransformShare.Instance.SharePosition();
                GameClient.Instance?.ClientPacketHandler.RequestMagicSkillUse(skill.Id, SkillIntention.CtrlPressed, SkillIntention.ShiftPressed);
                _stateMachine.SetWaitingForServerReply(true);
                break;
            case Event.SKILL_ALLOWED:
                _stateMachine.ChangeState(PlayerState.SKILL, arg0);
                break;
            case Event.SKILL_DENIED:
                break;
            case Event.ACTION_DENIED:
                break;
            case Event.DEAD:
                _stateMachine.ChangeState(PlayerState.DEAD);
                break;

        }
    }
}