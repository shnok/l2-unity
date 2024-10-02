using UnityEngine;

[RequireComponent(typeof(NetworkTransformReceive)),
    RequireComponent(typeof(NetworkCharacterControllerReceive))]
public class HumanoidNetworkEntity : NetworkEntity
{
    public override void Initialize()
    {
        base.Initialize();

        EntityLoaded = true;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        _animationController.SetAnimationProperty((int)HumanoidAnimationEvent.death, 1f, true);
    }

    public override bool StartAutoAttacking()
    {
        if (base.StartAutoAttacking())
        {
            _animationController.SetBool("atk01_" + _gear.WeaponAnim, true);
        }

        return true;
    }

    public override bool StopAutoAttacking()
    {
        if (base.StopAutoAttacking())
        {
            _animationController.SetBool("atk01_" + _gear.WeaponAnim, false);
            if (!_networkCharacterControllerReceive.IsMoving() && !IsDead())
            {
                _animationController.SetBool("atkwait_" + _gear.WeaponAnim, true);
            }
        }

        return true;
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        _audioHandler.PlaySound(EntitySoundEvent.Dmg);
    }

    public override void UpdateWaitType(ChangeWaitTypePacket.WaitType moveType)
    {
        base.UpdateWaitType(moveType);

        if (moveType == ChangeWaitTypePacket.WaitType.WT_SITTING)
        {
            _animationController.SetBool("sit", true);
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_STANDING)
        {
            _animationController.SetBool("stand", true);
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_START_FAKEDEATH)
        {
            _animationController.SetBool("death", true);
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_STOP_FAKEDEATH)
        {
        }
    }
}