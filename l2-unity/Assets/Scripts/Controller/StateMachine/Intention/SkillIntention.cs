using UnityEngine;

public class SkillIntention : IntentionBase
{
    private static Skill _lastSkillItention = null;

    public SkillIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        if (arg0 != null)
        {
            _lastSkillItention = (Skill)arg0;
        }

        if (_lastSkillItention == null)
        {
            return;
        }

        //TODO: Use skill levels skillgrp
        Skillgrp skillgrp = _lastSkillItention.Skillgrps[0];


        if (skillgrp.CastRange != -1) // target required?
        {
            Transform target = TargetManager.Instance.Target.transform;
            if (target == null)
            {
                return;
            }

            // TODO: If buff or heal and require target, if not target selected send skill on self

            float skillRange = skillgrp.CastRange / 52.5f * 0.95f; //5% error margin
            Entity targetEntity = TargetManager.Instance.Target;
            Vector3 targetPos = targetEntity.transform.position;

            float distance = Vector3.Distance(VectorUtils.To2D(PlayerEntity.Instance.transform.position), VectorUtils.To2D(targetPos));

            Debug.Log($"Distance: {distance} SkillRange: {skillRange}");
            if (distance <= skillRange && !_stateMachine.WaitingForServerReply)
            {
                Debug.LogWarning("Using skill on target");

                _stateMachine.ChangeState(PlayerState.IDLE);

                if (!targetEntity.IsDead)
                {
                    _stateMachine.NotifyEvent(Event.READY_TO_SKILL, _lastSkillItention);
                }
            }
            else
            {
                // Move to target with a 5% error margin
                PathFinderController.Instance.MoveTo(targetPos, skillRange, () =>
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_FOLLOW, MoveReason.SKILL);
                });
            }
        }
        else
        {
            _stateMachine.NotifyEvent(Event.READY_TO_SKILL, _lastSkillItention);
        }
    }

    public override void Exit() { }
    public override void Update()
    {

    }
}