public class NetworkMonsterCombat : NetworkCombat
{
    protected override void OnDeath()
    {
        base.OnDeath();
        AnimationController.SetAnimationProperty((int)MonsterAnimationEvent.Death, 1f, true);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        AudioHandler.PlaySound(EntitySoundEvent.Dmg);
    }

    public override bool StartAutoAttacking()
    {
        if (base.StartAutoAttacking())
        {
            AnimationController.SetAnimationProperty((int)MonsterAnimationEvent.Atk01, 1f);
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
                AnimationController.SetAnimationProperty((int)MonsterAnimationEvent.AtkWait, 1f);
            }
        }

        return true;
    }
}