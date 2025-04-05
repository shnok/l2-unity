using UnityEngine;

public class JumpingState : StateBase
{


    public JumpingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void HandleEvent(Event evt, object arg0)
    {

    }

    private void UpdateMoveAnimation()
    {
        NewPlayerAnimationController.Instance.Jump();
    }

    public override void Enter(object arg0)
    {
        UpdateMoveAnimation();
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
}