using System;

public class HumanoidStateAction : HumanoidStateBase
{
    public bool IsMoving()
    {
        return CharacterController.IsMoving();
    }

    public bool IsAttacking()
    {
        return AnimController.LastAnim == HumanoidAnimType.atk01;
        // return AnimController.GetBool(HumanoidAnimType.atk01);
    }

    public bool IsDead()
    {
        return Entity.IsDead;
    }

    protected bool ShouldAtkWait()
    {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (now - Entity.Combat.CombatTimestamp < 5000)
        {
            if (Entity.Combat.AttackTarget == null)
            {
                return true;
            }
        }

        return false;
    }

    protected bool DidAttackTimeout()
    {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // Check if AtkOnce was called, otherwise switch to AtkWait state
        // Add the attack duration and the last auto attack packet timestamp
        if (now > ((long)_referenceHolder.AnimationController.PAtkSpd) + _referenceHolder.Combat.CombatTimestamp + 150) //150 for acceptable ping delay
        {
            return true;
            // Debug.LogWarning("Should ATK WAIT! (StopAttack)");
        }

        return false;
    }
}
