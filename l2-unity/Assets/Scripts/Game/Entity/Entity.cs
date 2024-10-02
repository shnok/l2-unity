using System;
using UnityEngine;

[System.Serializable]
public class Entity : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected BaseAnimationController _animationController;
    [SerializeField] protected Gear _gear;
    [SerializeField] protected BaseAnimationAudioHandler _audioHandler;

    [Header("Entity")]
    [SerializeField] private bool _entityLoaded;
    [SerializeField] private NetworkIdentity _identity;
    [SerializeField] private Status _status;
    [SerializeField] private Stats _stats;
    [SerializeField] protected Appearance _appearance;
    [SerializeField] private CharacterRace _race;
    [SerializeField] private CharacterModelType _raceId;
    [SerializeField] private bool _running;
    [SerializeField] private bool _sitting;

    [Header("Combat")]
    [SerializeField] private int _targetId;
    [SerializeField] protected Transform _target;
    [SerializeField] protected Transform _attackTarget;
    [SerializeField] private long _stopAutoAttackTime;
    [SerializeField] private long _startAutoAttackTime;

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
    public CharacterModelType RaceId { get { return _raceId; } set { _raceId = value; } }
    public bool EntityLoaded { get { return _entityLoaded; } set { _entityLoaded = value; } }
    public Gear Gear { get { return _gear; } }
    public bool Running { get { return _running; } set { _running = value; } }

    public void FixedUpdate()
    {
        LookAtTarget();
    }

    protected virtual void LookAtTarget() { }

    public virtual void Initialize()
    {
        // TODO: Pre-assign components in prefabs
        if (_gear == null)
        {
            Debug.LogWarning($"[{transform.name}] Gear was not assigned, please pre-assign animator to avoid unecessary load.");
            TryGetComponent(out _gear);
        }

        if (_animationController == null)
        {
            Debug.LogWarning($"[{transform.name}] AnimationController was not assigned, please pre-assign animator to avoid unecessary load.");
            _animationController = GetComponentInChildren<BaseAnimationController>(true);
        }

        if (_audioHandler == null)
        {
            Debug.LogWarning($"[{transform.name}] AudioHandler was not assigned, please pre-assign animator to avoid unecessary load.");
            _audioHandler = GetComponentInChildren<BaseAnimationAudioHandler>(true);
        }

        UpdatePAtkSpeed(_stats.PAtkSpd);
        UpdateMAtkSpeed(_stats.MAtkSpd);
        UpdateRunSpeed(_stats.RunSpeed);
        UpdateWalkSpeed(_stats.WalkSpeed);

        EquipAllWeapons();
        EquipAllArmors();
    }

    public void EquipAllWeapons() { _gear.EquipAllWeapons(Appearance); }
    public void EquipAllArmors() { _gear.EquipAllArmors(Appearance); }

    // Called when ApplyDamage packet is received 
    public void ApplyDamage(int damage, bool criticalHit)
    {
        if (_status.Hp <= 0)
        {
            Debug.LogWarning("Trying to apply damage to a dead entity");
            return;
        }

        _status.Hp = Mathf.Max(_status.Hp - damage, 0);

        OnHit(criticalHit);

        if (_status.Hp <= 0)
        {
            OnDeath();
        }
    }

    /* Notify server that entity got attacked */
    public void InflictAttack(AttackType attackType)
    {
        GameClient.Instance.ClientPacketHandler.InflictAttack(_identity.Id, attackType);
    }

    protected virtual void OnDeath() { }

    protected virtual void OnHit(bool criticalHit)
    {
        // TODO: Add armor type for more hit sounds
        AudioManager.Instance.PlayHitSound(criticalHit, transform.position);
    }

    public virtual void OnStopMoving() { }

    public virtual void OnStartMoving(bool walking)
    {
        UpdateMoveType(!walking);
    }

    public virtual bool StartAutoAttacking()
    {
        if (_target == null)
        {
            Debug.LogWarning("Trying to attack a null target");
            return false;
        }

        _startAutoAttackTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _attackTarget = _target;

        return true;
    }

    public virtual bool StopAutoAttacking()
    {
        Debug.Log($"[{Identity.Name}] Stop autoattacking");
        if (_attackTarget == null)
        {
            return false;
        }

        _stopAutoAttackTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _attackTarget = null;
        return true;
    }

    public virtual float UpdatePAtkSpeed(int pAtkSpd)
    {
        _stats.PAtkSpd = pAtkSpd;

        float stat = StatsConverter.Instance.ConvertStat(Stat.PHYS_ATTACK_SPEED, pAtkSpd);
        _animationController.SetPAtkSpd(stat);

        return stat;
    }

    public virtual float UpdateMAtkSpeed(int mAtkSpd)
    {
        _stats.MAtkSpd = mAtkSpd;

        float stat = StatsConverter.Instance.ConvertStat(Stat.MAGIC_ATTACK_SPEED, mAtkSpd);
        _animationController.SetMAtkSpd(stat);

        return stat;
    }

    public virtual float UpdateRunSpeed(int speed)
    {
        float scaled = StatsConverter.Instance.ConvertStat(Stat.SPEED, speed);
        _stats.RunSpeed = speed;
        _stats.ScaledRunSpeed = scaled;

        float stat = StatsConverter.Instance.ConvertStat(Stat.SPEED, speed);
        _animationController.SetRunSpeed(stat);

        return stat;
    }

    public virtual float UpdateWalkSpeed(int speed)
    {
        float scaled = StatsConverter.Instance.ConvertStat(Stat.SPEED, speed);
        _stats.WalkSpeed = speed;
        _stats.ScaledWalkSpeed = scaled;

        float stat = StatsConverter.Instance.ConvertStat(Stat.SPEED, speed);

        _animationController.SetWalkSpeed(stat);

        return stat;
    }

    public bool IsDead()
    {
        return Status.Hp <= 0;
    }

    public virtual void UpdateWaitType(ChangeWaitTypePacket.WaitType moveType)
    {
        if (moveType == ChangeWaitTypePacket.WaitType.WT_SITTING)
        {
            _sitting = true;
        }
        else if (moveType == ChangeWaitTypePacket.WaitType.WT_STANDING)
        {
            _sitting = false;
        }
    }

    public virtual void UpdateMoveType(bool running)
    {
        Running = running;
    }
}
