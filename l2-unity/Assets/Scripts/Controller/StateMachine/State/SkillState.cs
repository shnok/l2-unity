
using UnityEngine;

public class SkillState : StateBase
{
    private Skill _currentSkill = null;

    public Skill CurrentSkill { get => _currentSkill; }

    public SkillState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object obj0)
    {
        if (obj0 != null)
        {
            _currentSkill = (Skill)obj0;
        }
    }

    public override void Update()
    {
        if (InputManager.Instance.Move)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_MOVE);
        }
        else if (TargetManager.Instance.HasAttackTarget() && TargetManager.Instance.AttackTarget.Status.IsDead)
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
        }
    }

    public override void HandleEvent(Event evt, object arg0)
    {
        switch (evt)
        {
            // Auto attack stop event
            case Event.CANCEL:
                _stateMachine.ChangeState(PlayerState.IDLE);

                if (_stateMachine.Intention == Intention.INTENTION_FOLLOW)
                {
                    // _stateMachine.ChangeIntention(Intention.INTENTION_ATTACK, AttackIntentionType.ChangeTarget);
                    _stateMachine.ChangeIntention(Intention.INTENTION_ATTACK);
                }
                break;
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