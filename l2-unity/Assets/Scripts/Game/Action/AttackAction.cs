using UnityEngine;
using static AttackingState;

public class AttackAction : L2Action
{
    public AttackAction() : base() { }

    // Local action
    public override void UseAction()
    {
        // If has a target and attack key pressed
        if (TargetManager.Instance.HasTarget())
        {
            // Debug.LogWarning("Use attack action.");

            Entity targetEntity = TargetManager.Instance.Target;
            EntityType targetType = targetEntity.Identity.EntityType;

            if ((targetType == EntityType.User || targetType == EntityType.NPC) && InputManager.Instance.Ctrl || targetType == EntityType.Monster)
            {
                //Todo: Check if the target is flagged or has karma too
                // TargetManager.Instance.SetAttackTarget();
                PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_ATTACK);
            }
            else
            {
                PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_INTERACT);
            }
        }
    }
}