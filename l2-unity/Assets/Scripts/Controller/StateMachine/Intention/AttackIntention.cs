using UnityEngine;

public class AttackIntention : IntentionBase
{
    public AttackIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        Transform target = TargetManager.Instance.Target.Data.ObjectTransform;

        if (target == null)
        {
            return;
        }

        if (_stateMachine.State == PlayerState.ATTACKING)
        {
            if (TargetManager.Instance.IsAttackTargetSet())
            {
                return;
            }
            else
            {
                _stateMachine.ChangeIntention(Intention.INTENTION_FOLLOW, MoveReason.ATTACK);
                return;
            }
        }

        Entity targetEntity = TargetManager.Instance.Target.Data.Entity;

        TargetManager.Instance.SetAttackTarget();
        float attackRange = WorldCombat.Instance.GetRealAttackRange(PlayerEntity.Instance, targetEntity);

        Vector3 targetPos = targetEntity.transform.position;
        float distance = Vector3.Distance(PlayerEntity.Instance.transform.position, targetPos);

        // Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

        // Is close enough? Is player already waiting for server reply?
        if (distance <= attackRange * 0.95f && !_stateMachine.WaitingForServerReply && !targetEntity.IsDead)
        {
            PlayerController.Instance.UpdateFinalAngleToLookAt(targetEntity.transform);

            _stateMachine.ChangeState(PlayerState.IDLE);

            _stateMachine.NotifyEvent(Event.READY_TO_ATTACK);
        }
        else
        {
            // Move to target with a 5% error margin

            MoveReason reason = MoveReason.ATTACK;

            if (targetEntity.IsDead)
            {
                attackRange = WorldCombat.Instance.GetInteractRange(PlayerEntity.Instance, targetEntity);
                reason = MoveReason.DEFAULT;
            }

            PathFinderController.Instance.MoveTo(targetPos, attackRange * 0.95f, () =>
            {
                _stateMachine.ChangeIntention(Intention.INTENTION_FOLLOW, reason);
            });
        }
    }

    public override void Exit() { }
    public override void Update()
    {

    }
}