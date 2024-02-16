using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkAnimationController)),
    RequireComponent(typeof(NetworkTransformReceive)),
    RequireComponent(typeof(NetworkCharacterControllerReceive)),
    RequireComponent(typeof(CharacterAnimationAudioHandler))]
public class NpcEntity : Entity {
    private CharacterAnimationAudioHandler _npcAnimationAudioHandler;

    protected override void Initialize() {
        base.Initialize();
        _npcAnimationAudioHandler = GetComponent<CharacterAnimationAudioHandler>();
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
        if(_networkAnimationReceive.GetAnimationProperty((int)NpcAnimationEvent.Atk01) == 0f) {
            _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Wait, 1f);
        }
    }

    public override void OnStartMoving(bool walking) {
        _networkAnimationReceive.SetAnimationProperty(walking ? (int)NpcAnimationEvent.Walk : (int)NpcAnimationEvent.Run, 1f);
    }

    public override bool StartAutoAttacking() {
        if (base.StartAutoAttacking()) {
            _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.Atk01, 1f);
        }

        return true;
    }

    public override bool StopAutoAttacking() {
        if (base.StopAutoAttacking()) {
            _networkAnimationReceive.SetAnimationProperty((int)NpcAnimationEvent.AtkWait, 1f);
        }

        return true;
    }

    public override float UpdateMAtkSpeed(int mAtkSpd) {
        float converted = base.UpdateMAtkSpeed(mAtkSpd);
        _networkAnimationReceive.SetMAtkSpd(converted);

        return converted;
    }

    public override float UpdatePAtkSpeed(int pAtkSpd) {
        float converted = base.UpdatePAtkSpeed(pAtkSpd);
        _networkAnimationReceive.SetPAtkSpd(converted);

        return converted;
    }

    public override float UpdateSpeed(int speed) {
        float converted = base.UpdateSpeed(speed);
        _networkAnimationReceive.SetMoveSpeed(converted);

        return converted;
    }
}
