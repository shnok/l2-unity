using System;
using UnityEngine;

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
        float now = Time.time;
        if (now > _referenceHolder.Combat.AttackEndTime) // + 150) //50 for acceptable ping delay
        {
            // Debug.LogWarning("Should ATK WAIT! (StopAttack)");
            return true;
        }

        return false;
    }
}
