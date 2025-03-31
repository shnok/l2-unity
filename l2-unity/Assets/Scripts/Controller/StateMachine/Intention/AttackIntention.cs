using UnityEngine;

public class AttackIntention : IntentionBase
{
    public AttackIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        Transform target = TargetManager.Instance.Target.transform;

        if (target == null)
        {
            return;
        }

        // if (_stateMachine.State == PlayerState.ATTACKING && TargetManager.Instance.IsAttackTargetSet())
        // {
        //     Debug.LogWarning("Attacking target is target");
        //     return;
        // }

        Entity targetEntity = TargetManager.Instance.Target;

        // TargetManager.Instance.SetAttackTarget();
        float attackRange = WorldCombat.Instance.GetRealAttackRange(PlayerEntity.Instance, targetEntity);

        Vector3 targetPos = targetEntity.transform.position;
        float distance = Vector3.Distance(PlayerEntity.Instance.transform.position, targetPos);

        // Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

        // Is close enough? Is player already waiting for server reply?
        if (targetEntity.IsDead)
        {
            attackRange = WorldCombat.Instance.GetInteractRange(PlayerEntity.Instance, targetEntity);
        }

        //TODO: Maybe avoid sending too many attack requests if already attacking and in range?

        if (distance <= attackRange * 0.95f && !_stateMachine.WaitingForServerReply)
        {
            // PlayerController.Instance.UpdateFinalAngleToLookAt(targetEntity.transform); -> Update angle once attack is allowed instead
            Debug.Log("Attacking a new target");

            _stateMachine.ChangeState(PlayerState.IDLE);

            if (!targetEntity.IsDead)
            {
                _stateMachine.NotifyEvent(Event.READY_TO_ATTACK);
            }
        }
        else
        {
            // Move to target with a 5% error margin

            MoveReason reason = MoveReason.ATTACK;

            if (targetEntity.IsDead)
            {
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