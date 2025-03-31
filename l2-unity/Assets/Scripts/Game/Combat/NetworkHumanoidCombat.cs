
// Used by NPCS and USERS
using UnityEngine;

public class NetworkHumanoidCombat : NetworkCombat
{
    public NewHumanoidAnimationController HumanoidAnimationController { get { return (NewHumanoidAnimationController)_referenceHolder.NewAnimationController; } }
    public HumanoidGear Gear { get { return (HumanoidGear)_referenceHolder.Gear; } }

    public override void OnDeath()
    {
        base.OnDeath();
        HumanoidAnimationController.Die();
    }

    public override void OnRevive()
    {
        base.OnRevive();
        HumanoidAnimationController.Wait(); //TODO: Revive anim
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

    public override bool AttackOnce(float hitTime, float atkEndTime, bool hitSuccess, Entity attackTarget)
    {
        if (base.AttackOnce(hitTime, atkEndTime, hitSuccess, attackTarget))
        {
            // HumanoidAnimationController.SetBool(HumanoidAnimType.atk01, true);
            HumanoidAnimationController.Attack();
            return true;
        }
        else
        {
            return false;
        }
    }
}