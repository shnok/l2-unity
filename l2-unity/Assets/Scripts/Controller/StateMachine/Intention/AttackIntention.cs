using UnityEngine;

public class AttackIntention : IntentionBase
{
    public AttackIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        Transform target = TargetManager.Instance.Target.Data.ObjectTransform;

        if (target == null)
        {
            Debug.Log("Target is null, CANCEL event sent");
            _stateMachine.NotifyEvent(Event.CANCEL);
            return;
        }

        TargetManager.Instance.SetAttackTarget();

        float attackRange = ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange;
        float distance = Vector3.Distance(PlayerEntity.Instance.transform.position, target.position);
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
            PathFinderController.Instance.MoveTo(target.position, ((PlayerStats)PlayerEntity.Instance.Stats).AttackRange * 0.9f);
        }
    }

    public override void Exit() { }
    public override void Update()
    {

    }
}