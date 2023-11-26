using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserEntity : Entity {
    [SerializeField]
    private NpcStatus status;
    public NpcStatus Status { get => status; set => status = value; }

    public override void ApplyDamage(byte attackId, int value) {
        throw new System.NotImplementedException();
    }

    public override void InflictAttack(AttackType attackType) {
        throw new System.NotImplementedException();
    }
}
