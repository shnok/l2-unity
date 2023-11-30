using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity {
    [SerializeField]
    private PlayerStatus status;
    public PlayerStatus Status { get => status; set => status = value; }


    private static PlayerEntity instance;
    public static PlayerEntity GetInstance() {
        return instance;
    }

    void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    public override void ApplyDamage(byte attackId, int value) {
        throw new System.NotImplementedException();
    }

    public override void InflictAttack(AttackType attackType) {
        throw new System.NotImplementedException();
    }
}