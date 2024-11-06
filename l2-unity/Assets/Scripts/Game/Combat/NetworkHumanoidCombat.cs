
// Used by NPCS and USERS
using UnityEngine;

public class NetworkHumanoidCombat : NetworkCombat
{
    public HumanoidAnimationController HumanoidAnimationController { get { return (HumanoidAnimationController)_referenceHolder.AnimationController; } }
    public HumanoidGear Gear { get { return (HumanoidGear)_referenceHolder.Gear; } }

    public override void OnDeath()
    {
        base.OnDeath();
        HumanoidAnimationController.SetBool(HumanoidAnimType.death, true);
    }

    public override void OnRevive()
    {
        base.OnRevive();
        HumanoidAnimationController.SetBool(HumanoidAnimType.wait, true);
    }

    protected override void OnHit(Hit hit)
    {
        base.OnHit(hit);
    }

    // public override void StartAttackStance()
    // {
    //     base.StartAttackStance();

    //     HumanoidAnimationController.SetBool(HumanoidAnimType.atk01, true);
    // }

    // public override void StopAttackStance()
    // {
    //     base.StopAttackStance();

    //     HumanoidAnimationController.SetBool(HumanoidAnimType.atk01, false);
    //     if (!NetworkCharacterControllerReceive.IsMoving() && !IsDead())
    //     {
    //         HumanoidAnimationController.SetBool(HumanoidAnimType.atkwait, true);
    //     }
    // }
    public override bool AttackOnce()
    {
        Debug.LogWarning("AttackOnce");
        if (base.AttackOnce())
        {
            HumanoidAnimationController.SetBool(HumanoidAnimType.atk01, true);
            return true;
        }
        else
        {
            return false;
        }
    }
}