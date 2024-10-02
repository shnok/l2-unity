using UnityEngine;

public class MonsterNetworkEntity : NetworkEntity
{
    public override void Initialize()
    {
        base.Initialize();

        EntityLoaded = true;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        _animationController.SetAnimationProperty((int)MonsterAnimationEvent.Death, 1f, true);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        _audioHandler.PlaySound(EntitySoundEvent.Dmg);
    }

    public override void OnStopMoving()
    {
        if (_animationController.GetAnimationProperty((int)MonsterAnimationEvent.Atk01) == 0f)
        {
            _animationController.SetAnimationProperty((int)MonsterAnimationEvent.Wait, 1f);
        }
    }

    public override void OnStartMoving(bool walking)
    {
        base.OnStartMoving(walking);
        _animationController.SetAnimationProperty(walking ? (int)MonsterAnimationEvent.Walk : (int)MonsterAnimationEvent.Run, 1f);
    }

    public override bool StartAutoAttacking()
    {
        if (base.StartAutoAttacking())
        {
            _animationController.SetAnimationProperty((int)MonsterAnimationEvent.Atk01, 1f);
        }

        if (networkCharacterController != null)
        {
            // Should stop moving if autoattacking
            networkCharacterController.SetDestination(transform.position);
        }

        return true;
    }

    public override bool StopAutoAttacking()
    {
        if (base.StopAutoAttacking())
        {
            if (!IsDead())
            {
                _animationController.SetAnimationProperty((int)MonsterAnimationEvent.AtkWait, 1f);
            }
        }

        return true;
    }
}
