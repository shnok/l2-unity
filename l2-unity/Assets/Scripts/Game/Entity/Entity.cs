using System;
using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour {
    [SerializeField] private bool _entityLoaded;
    [SerializeField] private NetworkIdentity _identity;
    [SerializeField] private Status _status;
    [SerializeField] private Stats _stats;
    [SerializeField] protected Appearance _appearance;

    [SerializeField] private CharacterRace _race;
    [SerializeField] private CharacterRaceAnimation _raceId;

    [Header("Combat")]
    [SerializeField] private int _targetId;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _attackTarget;
    [SerializeField] private long _stopAutoAttackTime;
    [SerializeField] private long _startAutoAttackTime;

    protected NetworkAnimationController _networkAnimationReceive;
    protected NetworkTransformReceive _networkTransformReceive;
    protected NetworkCharacterControllerReceive _networkCharacterControllerReceive;
    protected Gear _gear;

    public NetworkCharacterControllerReceive networkCharacterController { get { return _networkCharacterControllerReceive; } }
    public Status Status { get => _status; set => _status = value; }
    public Stats Stats { get => _stats; set => _stats = value; }
    public Appearance Appearance { get => _appearance; set { _appearance = value; } }
    public NetworkIdentity Identity { get => _identity; set => _identity = value; }
    public int TargetId { get => _targetId; set => _targetId = value; }
    public Transform Target { get { return _target; } set { _target = value; } }
    public Transform AttackTarget { get { return _attackTarget; } set { _attackTarget = value; } }
    public long StopAutoAttackTime { get { return _stopAutoAttackTime; } }
    public long StartAutoAttackTime { get { return _startAutoAttackTime; } }
    public CharacterRace Race { get { return _race; } set { _race = value; } }
    public CharacterRaceAnimation RaceId { get { return _raceId; } set { _raceId = value; } }
    public bool EntityLoaded { get { return _entityLoaded; } set { _entityLoaded = value; } }

    public void FixedUpdate() {
        LookAtTarget();
    }

    protected virtual void LookAtTarget() {
        if (AttackTarget != null && Status.Hp > 0) {
            _networkTransformReceive.LookAt(_attackTarget);
        }
    }

    public virtual void Initialize() {
        TryGetComponent(out _networkAnimationReceive);
        TryGetComponent(out _networkTransformReceive);
        TryGetComponent(out _networkCharacterControllerReceive);
        TryGetComponent(out _gear);

        UpdatePAtkSpeed(_stats.PAtkSpd);
        UpdateMAtkSpeed(_stats.MAtkSpd);
        UpdateSpeed(_stats.Speed);

        EquipAllWeapons();
    }

    // Called when ApplyDamage packet is received 
    public void ApplyDamage(int damage, int newHp, bool criticalHit) {
        if(_status.Hp <= 0) {
            Debug.LogWarning("Trying to apply damage to a dead entity");
            return;
        }

        _status.Hp = Mathf.Max(newHp, 0);

        OnHit(criticalHit);

        if(_status.Hp == 0) {
            OnDeath();
        }
    }

    protected virtual void EquipAllWeapons() {
        if(_gear == null) {
            Debug.LogWarning("Gear script is not attached to entity");
            return;
        }
        if (_appearance.LHand != 0) {
            _gear.EquipWeapon(_appearance.LHand, true);
        }
        if (_appearance.RHand != 0) {
            _gear.EquipWeapon(_appearance.RHand, false);
        }
    }

    /* Notify server that entity got attacked */
    public void InflictAttack(AttackType attackType) {
        ClientPacketHandler.Instance.InflictAttack(_identity.Id, attackType);
    }

    protected virtual void OnDeath() {
        if(_networkAnimationReceive != null) {
            _networkAnimationReceive.enabled = false;
        }
        if (_networkTransformReceive != null) {
            _networkTransformReceive.enabled = false;
        }
        if (_networkCharacterControllerReceive != null) {
            _networkCharacterControllerReceive.enabled = false;
        }
    }

    protected virtual void OnHit(bool criticalHit) {
        // TODO: Add armor type for more hit sounds
        AudioManager.Instance.PlayHitSound(criticalHit, transform.position);
    }

    public virtual void OnStopMoving() {}

    public virtual void OnStartMoving(bool walking) {}

    public virtual bool StartAutoAttacking() {
        if (_target == null) {
            Debug.LogWarning("Trying to attack a null target");
            return false;
        }

        _startAutoAttackTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _attackTarget = _target;

        return true;
    }

    public virtual bool StopAutoAttacking() {
        Debug.Log($"[{Identity.Name}] Stop autoattacking");
        if (_attackTarget == null) {
            Debug.Log("AttackTarget was null");
            return false;
        }

        _stopAutoAttackTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _attackTarget = null;
        return true;
    }

    public virtual float UpdatePAtkSpeed(int pAtkSpd) {
        _stats.PAtkSpd = pAtkSpd;
        return StatsConverter.Instance.ConvertStat(Stat.PHYS_ATTACK_SPEED, pAtkSpd);
    }

    public virtual float UpdateMAtkSpeed(int mAtkSpd) {
        _stats.MAtkSpd = mAtkSpd;
        return StatsConverter.Instance.ConvertStat(Stat.MAGIC_ATTACK_SPEED, mAtkSpd);
    }

    public virtual float UpdateSpeed(int speed) {
        float scaled = StatsConverter.Instance.ConvertStat(Stat.SPEED, speed);
        _stats.Speed = speed;
        _stats.ScaledSpeed = scaled;
        return StatsConverter.Instance.ConvertStat(Stat.SPEED, speed);
    }

    public bool IsDead() {
        return Status.Hp <= 0;
    }
}
