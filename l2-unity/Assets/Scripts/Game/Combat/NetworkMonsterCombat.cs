// Used by MONSTERS
using UnityEngine;

public class NetworkMonsterCombat : NetworkCombat
{
    public MonsterAnimationController MonsterAnimationController { get { return (MonsterAnimationController)_referenceHolder.AnimationController; } }

    protected override void OnDeath()
    {
        base.OnDeath();
        Debug.LogWarning("DEAD");
        MonsterAnimationController.SetBool(MonsterAnimationEvent.death, true);
    }

    protected override void OnHit(Hit hit)
    {
        base.OnHit(hit);
        //AudioHandler.PlaySound(EntitySoundEvent.Dmg);
    }

    public override bool StartAutoAttacking()
    {
        if (base.StartAutoAttacking())
        {
            MonsterAnimationController.SetBool(MonsterAnimationEvent.atk01, true);
        }

        if (NetworkCharacterControllerReceive != null)
        {
            // Should stop moving if autoattacking
            NetworkCharacterControllerReceive.SetDestination(transform.position);
        }

        return true;
    }

    public override bool StopAutoAttacking()
    {
        if (base.StopAutoAttacking())
        {
            if (!IsDead())
            {
                MonsterAnimationController.SetBool(MonsterAnimationEvent.atkwait, true);
            }
        }

        return true;
    }
}