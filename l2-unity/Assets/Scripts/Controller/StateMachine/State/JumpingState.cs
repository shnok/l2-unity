using UnityEngine;

public class JumpingState : StateBase
{
    public JumpingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        PlayerController.Instance.Jump();
        NewPlayerAnimationController.Instance.Jump();
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("JumpingState Exit");
    }

    public override void Update()
    {
        if (!PlayerController.Instance.IsJumping())
        {
            if (InputManager.Instance.Move)
            {
                _stateMachine.ChangeIntention(Intention.INTENTION_MOVE);
            }
            else
            {
                _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
            }
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