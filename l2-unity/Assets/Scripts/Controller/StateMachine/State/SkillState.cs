
using UnityEngine;

public class SkillState : StateBase
{
    private float _hitTime;
    public SkillState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object obj0)
    {
        if (obj0 != null)
        {
            _hitTime = (int)obj0 / 1000f + Time.time;
        }
    }

    public override void Update()
    {
        if (Time.time > _hitTime)
        {
            if (_stateMachine.Intention != Intention.INTENTION_MOVE_TO)
            {
                _stateMachine.ChangeState(PlayerState.IDLE);
            }

            return;
        }

        if (InputManager.Instance.CloseWindow)
        {
            _stateMachine.SetWaitingForServerReply(true);
            GameClient.Instance.ClientPacketHandler.SendRequestCancel(true);
        }
    }

    public override void HandleEvent(Event evt, object arg0)
    {
        switch (evt)
        {
            case Event.CANCEL:
                _stateMachine.ChangeState(PlayerState.IDLE, true);
                break;
            case Event.CLICK_TO_MOVE:
                _stateMachine.ChangeIntention(Intention.INTENTION_MOVE_TO, (Vector3)arg0);
                break;
            case Event.ACTION_ALLOWED:
                NetworkCharacterControllerShare.Instance.ForceShareMoveDirection();
                if (_stateMachine.Intention == Intention.INTENTION_MOVE_TO)
                {
                    if (!PlayerController.Instance.IntentionToRun)
                    {
                        _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                        return;
                    }

                    //Set state as running first to change to movable state
                    _stateMachine.ChangeState(PlayerState.MOVING);

                    _stateMachine.ChangeIntention(Intention.INTENTION_MOVE_TO); //not giving an argument will use last position as destination
                }
                break;
            case Event.DEAD:
                _stateMachine.ChangeState(PlayerState.DEAD);
                break;
        }
    }

    public override void Exit()
    {
        PlayerController.Instance.StopLookAt();
    }
}