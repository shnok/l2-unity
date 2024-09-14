using UnityEngine;

public class SittingState : StateBase
{
    public SittingState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public const float SIT_ANIM_LENGTH_SEC = 4.5f;
    private bool _sitting = false;

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
        _sitting = false;
        _enterTime = Time.time;
    }

    public override void Update()
    {
        if (Time.time - _enterTime >= SIT_ANIM_LENGTH_SEC && !_sitting)
        {
            _sitting = true;
            _stateMachine.ChangeState(PlayerState.SIT_WAIT);
        }
    }
}