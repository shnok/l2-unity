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

    public override void OnStopMoving() {
        _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Wait, 1f);
    }

    public override void OnStartMoving(bool walking) {
        _networkAnimationReceive.SetAnimationProperty(walking ? (int)MonsterAnimationEvent.Walk : (int)MonsterAnimationEvent.Run, 1f);
    }

    public override bool StartAutoAttacking() {
        if (base.StartAutoAttacking()) {
            _networkAnimationReceive.SetAnimationProperty((int)MonsterAnimationEvent.Atk01, 1f);
        }

        return true;
    }

    public override bool StopAutoAttacking() {
        if (base.StopAutoAttacking()) {
            _networkAnimationReceive.SetAnimationProperty((int)MonsterAnimationEvent.AtkWait, 1f);
        }

        return true;
    }
}
