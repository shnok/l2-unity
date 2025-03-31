// Used by MONSTERS
using UnityEngine;

public class NetworkMonsterCombat : NetworkCombat
{
    public NewMonsterAnimationController MonsterAnimationController { get { return (NewMonsterAnimationController)_referenceHolder.NewAnimationController; } }

    public override void OnDeath()
    {
        base.OnDeath();
        MonsterAnimationController.Die();
    }

    public override void OnRevive()
    {
        base.OnRevive();
        MonsterAnimationController.Wait();
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

    public override bool AttackOnce(float hitTime, float atkEndTime, bool hitSuccess, Entity attackTarget)
    {
        if (base.AttackOnce(hitTime, atkEndTime, hitSuccess, attackTarget))
        {
            MonsterAnimationController.Attack();
            return true;
        }
        else
        {
            return false;
        }
    }
}