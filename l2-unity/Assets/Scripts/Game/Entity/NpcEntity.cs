using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkAnimationReceive)),
    RequireComponent(typeof(NetworkTransformReceive)),
    RequireComponent(typeof(NetworkCharacterControllerReceive)),
    RequireComponent(typeof(NpcAnimationAudioHandler))]
public class NpcEntity : Entity {
    private NpcAnimationAudioHandler _npcAnimationAudioHandler;

    protected override void Initialize() {
        base.Initialize();
        _npcAnimationAudioHandler = GetComponent<NpcAnimationAudioHandler>();
    }

    protected override void OnDeath() {
        base.OnDeath();
        _networkAnimationReceive.SetAnimationProperty((int) NpcAnimationEvent.Death, 1f, true);
    }

    protected override void OnHit(bool criticalHit) {
        base.OnHit(criticalHit);
        _npcAnimationAudioHandler.PlaySound(CharacterSoundEvent.Dmg);
    }

    public override void OnStopMoving() {
        _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Wait, 1f);
    }

    public override void OnStartMoving(bool walking) {
        _networkAnimationReceive.SetAnimationProperty(walking ? (int)NpcAnimationEvent.Walk : (int)NpcAnimationEvent.Run, 1f);
    }

}
