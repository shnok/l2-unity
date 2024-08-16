using UnityEngine;

[RequireComponent(typeof(NetworkAnimationController)), 
    RequireComponent(typeof(NetworkTransformReceive)), 
    RequireComponent(typeof(NetworkCharacterControllerReceive)), 
    RequireComponent(typeof(MonsterAnimationAudioHandler))]
public class MonsterEntity : Entity {
    private MonsterAnimationAudioHandler _monsterAnimationAudioHandler;

    [SerializeField] private NpcData _npcData;

    public NpcData NpcData { get { return _npcData; } set { _npcData = value; } }

    public override void Initialize() {
        base.Initialize();
        _monsterAnimationAudioHandler = GetComponent<MonsterAnimationAudioHandler>();

        EntityLoaded = true;
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
        if (_networkAnimationReceive.GetAnimationProperty((int)MonsterAnimationEvent.Atk01) == 0f) {
            _networkAnimationReceive.SetAnimationProperty((int)MonsterAnimationEvent.Wait, 1f);
        }
    }

    public override void OnStartMoving(bool walking) {
        _networkAnimationReceive.SetAnimationProperty(walking ? (int)MonsterAnimationEvent.Walk : (int)MonsterAnimationEvent.Run, 1f);
    }

    public override bool StartAutoAttacking() {
        if (base.StartAutoAttacking()) {
            _networkAnimationReceive.SetAnimationProperty((int)MonsterAnimationEvent.Atk01, 1f);
        }

        if (networkCharacterController != null) {
            // Should stop moving if autoattacking
            networkCharacterController.SetDestination(transform.position);
        }

        return true;
    }

    public override bool StopAutoAttacking() {
        if (base.StopAutoAttacking()) {
            if(!IsDead()) {
                _networkAnimationReceive.SetAnimationProperty((int)MonsterAnimationEvent.AtkWait, 1f);
            }
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
