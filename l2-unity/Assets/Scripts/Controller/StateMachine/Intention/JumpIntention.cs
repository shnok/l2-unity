using UnityEngine;

// Click to move intention
public class JumpIntention : IntentionBase
{

    public JumpIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        _stateMachine.ChangeState(PlayerState.JUMPING);
    }
    public override void Exit() { }
    public override void Update()
    {
        _stateMachine.ChangeState(PlayerState.JUMPING);
    }
}