using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkAnimationReceive)),
    RequireComponent(typeof(NetworkTransformReceive)),
    RequireComponent(typeof(NetworkCharacterControllerReceive))]

public class UserEntity : Entity {

    protected override void Initialize() {
        base.Initialize();
    }

    protected override void OnDeath() {
        base.OnDeath();
    }

    protected override void OnHit() {
    }
}