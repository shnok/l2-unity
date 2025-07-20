
using UnityEngine;

public class SkillState : StateBase
{
    private float _hitTime;
    public SkillState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    private bool _intentionToRun;
    private Vector3 _intentionToRunDestination;

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
                _stateMachine.ChangeIntention(Intention.INTENTION_IDLE, false);
            }

            return;
        }

        if (InputManager.Instance.CloseWindow || InputManager.Instance.Move || InputManager.Instance.Jump)
        {
            TryCancelCast();
        }
    }

    private void TryCancelCast()
    {
        if (_stateMachine.WaitingForServerReply)
        {
            return;
        }

        _stateMachine.SetWaitingForServerReply(true);
        GameClient.Instance.ClientPacketHandler.SendRequestCancel(true);
    }

    public override void HandleEvent(Event evt, object arg0)
    {
        switch (evt)
        {
            case Event.CANCEL:
                Debug.LogWarning("Cast canceled.");
                if (_intentionToRun)
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_MOVE_TO, _intentionToRunDestination);
                }
                else
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_IDLE, true);
                }
                break;
            case Event.CLICK_TO_MOVE:
                _intentionToRun = true;
                _intentionToRunDestination = (Vector3)arg0;
                TryCancelCast();
                break;
            case Event.ACTION_ALLOWED:
                Debug.LogWarning("Move allowed.");
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
            case Event.ACTION_DENIED:
                Debug.LogWarning("Failed to cancel cast.");
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