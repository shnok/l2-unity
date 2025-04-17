using System;
using UnityEngine;

// Used by LOCAL PLAYER
[System.Serializable]
public class PlayerCombat : Combat
{
    private static PlayerCombat _instance;
    public static PlayerCombat Instance { get => _instance; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        PlayerStateMachine.Instance.NotifyEvent(Event.DEAD);
    }

    public override void OnRevive()
    {
        base.OnRevive();
        PlayerStateMachine.Instance.NotifyEvent(Event.REVIVED);
    }

    protected override void OnHit(Hit hit)
    {
        base.OnHit(hit);
    }

    public override bool AttackOnce(float hitTime, float atkEndTime, bool hitSuccess, Entity attackTarget)
    {
        if (base.AttackOnce(hitTime, atkEndTime, hitSuccess, attackTarget))
        {
            PlayerStateMachine.Instance.OnAttackAllowed();
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void LookAtTarget()
    {
        Transform target = GetTargetToLookAt();
        if (target != null && target != transform)
        {
            PlayerController.Instance.UpdateFinalAngleToLookAt(target);
        }
    }

    protected override void SetAttackTarget(Entity attackTarget)
    {
        base.SetAttackTarget(attackTarget);
        TargetManager.Instance.SetAttackTarget(attackTarget);
    }
}
