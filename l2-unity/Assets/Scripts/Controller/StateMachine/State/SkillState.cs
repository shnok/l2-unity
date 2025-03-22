
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
        // if (InputManager.Instance.Move)
        // {
        //     _stateMachine.ChangeIntention(Intention.INTENTION_MOVE);
        // }

        if (Time.time > _hitTime)
        {
            _stateMachine.ChangeState(PlayerState.IDLE);
        }
    }

    public override void HandleEvent(Event evt, object arg0)
    {
        switch (evt)
        {
            case Event.DEAD:
                _stateMachine.ChangeState(PlayerState.DEAD);
                break;
        }
    }

    public override void Exit()
    {
        // PlayerCombat.Instance.StopAttackStance();
        PlayerController.Instance.StopLookAt();
    }
}