using UnityEngine;
using static AttackingState;

public class AttackIntention : IntentionBase
{
    public AttackIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        Transform target = TargetManager.Instance.Target.Data.ObjectTransform;

        if (target == null)
        {
            Debug.Log("Target is null, CANCEL event sent");
            // _stateMachine.NotifyEvent(Event.CANCEL);
            return;
        }

        if (_stateMachine.State == PlayerState.ATTACKING)
        {
            if (TargetManager.Instance.IsAttackTargetSet())
            {
                // Already attacking target
                return;
            }
            else
            {
                // if (!_stateMachine.WaitingForServerReply)
                // {
                //     _stateMachine.SetWaitingForServerReply(true);
                //     GameClient.Instance.ClientPacketHandler.UpdateMoveDirection(Vector3.zero);
                // }

                _stateMachine.ChangeIntention(Intention.INTENTION_FOLLOW);

                return;
            }
        }

        AttackIntentionType type = (AttackIntentionType)arg0;

        Debug.LogWarning((AttackIntentionType)arg0);

        if (type != AttackIntentionType.TargetReached)
        {
            TargetManager.Instance.SetAttackTarget();
        }

        Vector3 targetPos = TargetManager.Instance.AttackTarget.Data.ObjectTransform.position;

        float attackRange = ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange;
        float distance = Vector3.Distance(PlayerEntity.Instance.transform.position, targetPos);
        Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

        // Is close enough? Is player already waiting for server reply?
        if (distance <= attackRange * 0.9f && !_stateMachine.WaitingForServerReply)
        {
            _stateMachine.ChangeState(PlayerState.IDLE);
            _stateMachine.NotifyEvent(Event.READY_TO_ACT);
        }
        else
        {
            // Move to target with a 10% error margin
            PathFinderController.Instance.MoveTo(targetPos, ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange * 0.9f);
        }
    }

    public override void Exit() { }
    public override void Update()
    {

    }
}