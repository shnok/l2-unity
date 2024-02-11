using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    void Update()
    {
        if(InputManager.Instance.IsInputPressed(InputType.Attack)) {
            // for debug purpose
            if(TargetManager.Instance.HasTarget()) {
                WorldCombat.Instance.ApplyDamage(
                    transform, 
                    TargetManager.Instance.GetTargetData().Data.ObjectTransform, 
                    (byte) AttackType.AutoAttack, 
                    1);
            }
        }
    }
}
