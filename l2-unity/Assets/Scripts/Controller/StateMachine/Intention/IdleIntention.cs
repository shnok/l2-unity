using UnityEngine;

public class IdleIntention : IntentionBase
{
    public IdleIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        // if (_stateMachine.State != PlayerState.ATTACKING)
        // {
        // IdleIntention is not triggerable by player, no need to ask server for permission
        _stateMachine.ChangeState(PlayerState.IDLE, arg0);
        // }
    }

    public override void Exit() { }
    public override void Update()
    {
        // if (_stateMachine.State == PlayerState.ATTACKING)
        // {
        //     if (Time.time >= PlayerCombat.Instance.AttackEndTime)
        //         _stateMachine.ChangeState(PlayerState.IDLE);
        // }
    }
}