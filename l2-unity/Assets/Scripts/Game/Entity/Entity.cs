using System;
using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour {
    [SerializeField] private NetworkIdentity _identity;
    [SerializeField] private Status _status;
    [SerializeField] private int _targetId;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _attackTarget;
    [SerializeField] private long _stopAutoAttackTime;
    [SerializeField] private long _startAutoAttackTime;

    protected NetworkAnimationController _networkAnimationReceive;
    private NetworkTransformReceive _networkTransformReceive;
    private NetworkCharacterControllerReceive _networkCharacterControllerReceive;

    public Status Status { get => _status; set => _status = value; }
    public NetworkIdentity Identity { get => _identity; set => _identity = value; }
    public int TargetId { get => _targetId; set => _targetId = value; }
    public Transform Target { get { return _target; } set { _target = value; } }
    public Transform AttackTarget { get { return _attackTarget; } set { _attackTarget = value; } }
    public long StopAutoAttackTime { get { return _stopAutoAttackTime; } }
    public long StartAutoAttackTime { get { return _startAutoAttackTime; } }

    public void Start() {
        Initialize();
    }

    public void FixedUpdate() {
        LookAtTarget();
    }

    protected virtual void LookAtTarget() {
        if (AttackTarget != null) {
            transform.LookAt(new Vector3(AttackTarget.position.x, transform.position.y, AttackTarget.position.z));
        }
    }

    protected virtual void Initialize() {
        TryGetComponent(out _networkAnimationReceive);
        TryGetComponent(out _networkTransformReceive);
        TryGetComponent(out _networkCharacterControllerReceive);

        UpdatePAtkSpeed(_status.PAtkSpd);
        UpdateMAtkSpeed(_status.PAtkSpd);
        UpdateSpeed(_status.Speed);
    }

    /* Called when ApplyDamage packet is received */
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

        if (_networkCharacterControllerReceive != null) {
            // Should stop moving if autoattacking
            _networkCharacterControllerReceive.SetDestination(transform.position);
        }

        return true;
    }

    public virtual bool StopAutoAttacking() {
        Debug.Log("Stop autoattacking");
        if (_attackTarget == null) {
            return false;
        }

        _stopAutoAttackTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _attackTarget = null;
        return true;
    }

    public virtual float UpdatePAtkSpeed(int pAtkSpd) {
        Status.PAtkSpd = pAtkSpd;
        return StatsConverter.Instance.ConvertStat(Stat.PHYS_ATTACK_SPEED, pAtkSpd);
    }

    public virtual float UpdateMAtkSpeed(int mAtkSpd) {
        Status.MAtkSpd = mAtkSpd;
        return StatsConverter.Instance.ConvertStat(Stat.MAGIC_ATTACK_SPEED, mAtkSpd);
    }

    public virtual float UpdateSpeed(int speed) {
        float scaled = StatsConverter.Instance.ConvertStat(Stat.SPEED, speed);
        Status.Speed = speed;
        Status.ScaledSpeed = scaled;
        return StatsConverter.Instance.ConvertStat(Stat.SPEED, speed);
    }

    public bool IsDead() {
        return Status.Hp <= 0;
    }
}
