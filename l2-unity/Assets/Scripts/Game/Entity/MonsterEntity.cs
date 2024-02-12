using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkAnimationReceive)), 
    RequireComponent(typeof(NetworkTransformReceive)), 
    RequireComponent(typeof(NetworkCharacterControllerReceive)), 
    RequireComponent(typeof(MonsterAnimationAudioHandler))]
public class MonsterEntity : Entity {
    private MonsterAnimationAudioHandler _monsterAnimationAudioHandler;

    protected override void Initialize() {
        base.Initialize();
        _monsterAnimationAudioHandler = GetComponent<MonsterAnimationAudioHandler>();
    }

    protected override void OnDeath() {
        base.OnDeath();
        _networkAnimationReceive.SetAnimationProperty((int)MonsterAnimationEvent.Death, 1f, true);
    }

    protected override void OnHit(bool criticalHit) {
        base.OnHit(criticalHit);
        _monsterAnimationAudioHandler.PlaySound(MonsterSoundEvent.Dmg);
    }
}
