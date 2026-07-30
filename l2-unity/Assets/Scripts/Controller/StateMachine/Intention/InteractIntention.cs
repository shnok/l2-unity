using UnityEngine;

public class InteractIntention : IntentionBase
{
    public InteractIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        Transform target = TargetManager.Instance.Target.transform;

        if (target == null)
        {
            return;
        }

        Entity targetEntity = TargetManager.Instance.Target;
        float interactRange = WorldCombat.Instance.GetInteractRange(PlayerEntity.Instance, targetEntity);
        Vector3 targetPos = targetEntity.transform.position;
        float distance = Vector3.Distance(PlayerEntity.Instance.transform.position, targetPos);

        // Debug.Log($"target: {target} distance: {distance} range: {attackRange}");

        if (distance <= interactRange * 0.95f)
        {
            PlayerController.Instance.UpdateFinalAngleToLookAt(targetEntity.transform);
            _stateMachine.ChangeState(PlayerState.IDLE);
            _stateMachine.NotifyEvent(Event.READY_TO_INTERACT);
        }
        else
        {
            // Move to target with a 5% error margin
            PathFinderController.Instance.MoveTo(targetPos, interactRange * 0.95f, () =>
            {
                _stateMachine.ChangeIntention(Intention.INTENTION_FOLLOW, MoveReason.INTERACT);
            });
        }
    }

    public override void Exit() { }
    public override void Update() { }
}