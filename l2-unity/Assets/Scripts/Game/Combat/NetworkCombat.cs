using UnityEngine;

public abstract class NetworkCombat : Combat
{
    [SerializeField] protected bool _attackStance;

    protected NetworkEntityReferenceHolder ReferenceHolder { get { return (NetworkEntityReferenceHolder)_referenceHolder; } }
    protected NetworkTransformReceive NetworkTransformReceive { get { return ReferenceHolder.NetworkTransformReceive; } }
    protected NetworkCharacterControllerReceive NetworkCharacterControllerReceive { get { return ReferenceHolder.NetworkCharacterControllerReceive; } }
    protected NetworkIdentity Identity { get { return _referenceHolder.Entity.Identity; } }
    public bool AttackStance { get { return _attackStance; } set { _attackStance = value; } }

    public override void OnDeath()
    {
        base.OnDeath();

        if (AnimationController != null)
        {
            AnimationController.enabled = false;
        }
        if (NetworkTransformReceive != null)
        {
            NetworkTransformReceive.enabled = false;
        }
        if (NetworkCharacterControllerReceive != null)
        {
            NetworkCharacterControllerReceive.enabled = false;
        }

        ReferenceHolder.ClickArea.transform.localPosition = Vector3.zero;
        Vector3 scale = ReferenceHolder.ClickArea.transform.localScale;
        ReferenceHolder.ClickArea.transform.localScale = new Vector3(scale.x, ReferenceHolder.Entity.Appearance.CollisionHeight, scale.z);
    }

    public override void OnRevive()
    {
        base.OnRevive();

        if (AnimationController != null)
        {
            AnimationController.enabled = true;
        }
        if (NetworkTransformReceive != null)
        {
            NetworkTransformReceive.enabled = true;
        }
        if (NetworkCharacterControllerReceive != null)
        {
            NetworkCharacterControllerReceive.enabled = true;
        }
    }

    public void LookAtTarget()
    {
        Debug.LogWarning(AttackTarget);
        if (AttackTarget != null && !Status.IsDead)
        {
            NetworkTransformReceive.LookAt(_attackTarget.transform);
        }
    }

    public override void OnStopMoving()
    {
        if (_attackStance)
        {
            //Refresh autoattack animation
            // Debug.LogWarning($"[{transform.name}] Reached destination resuming autoattack animation");

            // if (AttackTarget != null && !AttackTarget.ReferenceHolder.Combat.IsDead())
            // {
            //     StartAutoAttacking();
            // }
            // else
            // {
            //     StopAutoAttacking();
            // }
        }
    }

    // public override void StartAttackStance()
    // {
    //     base.StartAttackStance();

    //     // LookAtTarget();

    //     // Debug.LogWarning($"[{transform.name}] StartAutoattacking");

    //     // _attackStance = true;

    //     // if (NetworkCharacterControllerReceive != null)
    //     // {
    //     //     // Should stop moving if autoattacking
    //     //     NetworkCharacterControllerReceive.SetDestination(
    //     //         transform.position,
    //     //         WorldCombat.Instance.GetRealAttackRange(_referenceHolder.Entity, AttackTarget));
    //     // }
    // }

    // public override void StopAttackStance()
    // {
    //     // base.StopAttackStance();

    //     // Debug.LogWarning($"[{transform.name}] StopAutoattacking");

    //     // _attackStance = false;
    // }

    public override bool AttackOnce(float hitTime)
    {
        if (base.AttackOnce(hitTime))
        {
            Debug.Log("Look At Target");
            LookAtTarget();
            return true;
        }
        else
        {
            return false;
        }
    }
}