using UnityEngine;

// Click to move intention
public class MoveToIntention : IntentionBase
{
    public static MoveReason MoveReason { get; private set; }

    // Last click to move desired location
    public static Vector3 _lastClickToMoveLocation;

    public MoveToIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        MoveReason = MoveReason.CLICK_TO_MOVE;

        if (arg0 != null && arg0 is Vector3)
        {
            _lastClickToMoveLocation = (Vector3)arg0;
        }

        if (_stateMachine.State == PlayerState.SITTING || _stateMachine.State == PlayerState.SIT_WAIT || _stateMachine.State == PlayerState.STANDING)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_STAND);
            return;
        }

        PlayerController.Instance.IntentionToRun = true;

        if (_stateMachine.IsInMovableState())
        {
            //Vector3 is given when using click to move
            if (_lastClickToMoveLocation != Vector3.zero)
            {
                PathFinderController.Instance.MoveTo(_lastClickToMoveLocation, () =>
                {
                    RunOrWalk(MoveReason);
                });
            }
            else
            {
                _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
            }
        }
        else if (!_stateMachine.WaitingForServerReply)
        {
            _stateMachine.SetWaitingForServerReply(true);
            NetworkCharacterControllerShare.Instance.ForceShareMoveDirection();
        }
        else
        {
            PlayerController.Instance.StopMoving();
        }
    }

    private void RunOrWalk(MoveReason moveReason)
    {
        _stateMachine.ChangeState(PlayerState.MOVING, moveReason);
    }

    public override void Exit() { }
    public override void Update()
    {
    }
}