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

    protected override void OnHit() {
        _npcAnimationAudioHandler.PlaySound(CharacterSoundEvent.Dmg);
    }
}
