using UnityEngine;

public class StandingState : StateBase
{
    public const float STAND_ANIM_LENGTH_SEC = 5f;
    private bool _sitting = true;

    public StandingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.DEAD:
                break;
        }
    }

    private float _enterTime;
    public override void Enter()
    {
        _sitting = true;
        _enterTime = Time.time;
    }

    public override void Update()
    {
        if (Time.time - _enterTime >= STAND_ANIM_LENGTH_SEC && _sitting)
        {
            _sitting = false;
            _stateMachine.ChangeState(PlayerState.IDLE);
        }
    }
}