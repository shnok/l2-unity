// Used by MONSTERS
using UnityEngine;

public class NetworkMonsterCombat : NetworkCombat
{
    public MonsterAnimationController MonsterAnimationController { get { return (MonsterAnimationController)_referenceHolder.AnimationController; } }

    public override void OnDeath()
    {
        base.OnDeath();
        MonsterAnimationController.SetBool(MonsterAnimationEvent.death, true);
    }

    public override void OnRevive()
    {
        base.OnRevive();
        MonsterAnimationController.SetBool(MonsterAnimationEvent.wait, true);
    }

    protected override void OnHit(Hit hit)
    {
        base.OnHit(hit);
    }

    // public override void StartAttackStance()
    // {
    //     base.StartAttackStance();

    //     // MonsterAnimationController.SetBool(MonsterAnimationEvent.atk01, true);
    // }

    // public override void StopAttackStance()
    // {
    //     base.StopAttackStance();

    //     // if (!IsDead())
    //     // {
    //     //     MonsterAnimationController.SetBool(MonsterAnimationEvent.atkwait, true);
    //     // }
    // }

    public override bool AttackOnce(float hitTime, bool hitSuccess, Entity attackTarget)
    {
        Debug.LogWarning("AttackOnce");
        if (base.AttackOnce(hitTime, hitSuccess, attackTarget))
        {
            MonsterAnimationController.SetBool(MonsterAnimationEvent.atk01, true);
            return true;
        }
        else
        {
            return false;
        }
    }
}