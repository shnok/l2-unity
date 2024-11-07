using UnityEngine;

public class IdleIntention : IntentionBase
{
    public IdleIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        // IdleIntention is not triggerable by player, no need to ask server for permission
        _stateMachine.ChangeState(PlayerState.IDLE);
    }

    public override void Exit() { }
    public override void Update()
    {

    }
}