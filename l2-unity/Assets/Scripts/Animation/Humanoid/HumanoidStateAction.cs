using System;

public class HumanoidStateAction : HumanoidStateBase
{
    public bool IsMoving()
    {
        return _networkCharacterControllerReceive.IsMoving();
    }

    public bool IsAttacking()
    {
        return _animator.GetBool("atk01_" + _gear.WeaponAnim) ||
        _animator.GetNextAnimatorStateInfo(0).IsName("atk01_" + _gear.WeaponAnim);
    }

    protected bool ShouldAtkWait()
    {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (now - _entity.StopAutoAttackTime < 5000)
        {
            if (_entity.AttackTarget == null)
            {
                return true;
            }
        }

        return false;
    }
}
