using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateAction : UserStateBase
{
    public bool IsMoving() {
        return !VectorUtils.IsVectorZero2D(_networkCharacterControllerReceive.MoveDirection); 
    }

    protected bool ShouldAtkWait() {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (now - _entity.StopAutoAttackTime < 5000) {
            if (_entity.AttackTarget == null) {
                SetBool("atkwait" + _weaponType.ToString(), true);
                return true;
            }
        }

        return false;
    }
}
