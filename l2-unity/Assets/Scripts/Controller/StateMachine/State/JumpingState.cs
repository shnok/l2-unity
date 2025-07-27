using UnityEngine;

public class JumpingState : StateBase
{
    public JumpingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private PlayerState _stateBeforeJump;

    public override void Enter(object arg0)
    {
        _stateBeforeJump = (PlayerState)arg0;

        if (!PlayerController.Instance.IsJumping())
        {
            PlayerController.Instance.Jump();
            NewPlayerAnimationController.Instance.Jump();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        if (!PlayerController.Instance.IsJumping())
        {
            _stateMachine.ChangeState(_stateBeforeJump);
        }
    }

    public override void HandleEvent(Event evt, object arg0)
    {
        switch (evt)
        {
            case Event.CLICK_TO_MOVE:
                _stateMachine.ChangeIntention(Intention.INTENTION_MOVE_TO, (Vector3)arg0);
                break;
        }
    }
}