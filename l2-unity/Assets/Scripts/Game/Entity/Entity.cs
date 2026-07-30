using System;
using UnityEngine;

[System.Serializable]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected EntityReferenceHolder _referenceHolder;

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

    public Status Status { get => _status; set => _status = value; }
    public Stats Stats { get => _stats; set => _stats = value; }
    public Appearance Appearance { get => _appearance; private set { _appearance = value; } }
    public NetworkIdentity Identity { get => _identity; set => _identity = value; }

    public CharacterRace Race { get { return _race; } set { _race = value; } }
    public CharacterModelType RaceId { get { return _raceId; } set { _raceId = value; } }
    public bool EntityLoaded { get { return _entityLoaded; } set { _entityLoaded = value; } }
    public bool Running { get { return _running; } set { _running = value; } }
    public NewBaseAnimationController AnimationController { get { return _referenceHolder.NewAnimationController; } }
    public EntityReferenceHolder ReferenceHolder { get { return _referenceHolder; } }
    public Gear Gear { get { return _referenceHolder.Gear; } }
    public Combat Combat { get { return _referenceHolder.Combat; } }
    public bool IsDead { get { return Status.IsDead; } }
    public bool IsSitting { get { return _sitting; } }

    private void Awake()
    {
        if (_referenceHolder == null)
        {
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
            _referenceHolder = GetComponent<EntityReferenceHolder>();
        }
    }

    public void FixedUpdate()
    {
        LookAtTarget();
    }

    protected virtual void LookAtTarget() { }

    public virtual void Initialize()
    {
        if (_referenceHolder == null)
        {
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
            _referenceHolder = GetComponent<EntityReferenceHolder>();
        }

        // UpdatePAtkSpeed(_stats.PAtkSpd);
        // UpdateMAtkSpeed(_stats.MAtkSpd);
        // UpdateRunSpeed(_stats.RunSpeed);
        // UpdateWalkSpeed(_stats.WalkSpeed);

        // EquipAllWeapons();
        // EquipAllArmors();
    }

    // public void EquipAllWeapons() { Gear.EquipAllWeapons(Appearance); }
    // public void EquipAllArmors() { Gear.EquipAllArmors(Appearance); }

    public virtual void OnStopMoving()
    {
        // Debug.LogWarning("On stop moving base");
        AnimationController.Wait();
    }

    public virtual void OnStartMoving(bool walking)
    {
        UpdateMoveType(!walking);
    }

    public virtual float UpdatePAtkSpeed(int pAtkSpd)
    {
        if (_stats.PAtkSpd == pAtkSpd)
        {
            return 0;
        }

        _stats.PAtkSpd = pAtkSpd;

        float stat = StatsConverter.Instance.ConvertStat(Stat.PHYS_ATTACK_SPEED, pAtkSpd);

        AnimationController.SetPAtkSpd(stat);

        return stat;
    }

    public virtual float UpdateMAtkSpeed(int mAtkSpd)
    {
        if (_stats.MAtkSpd == mAtkSpd)
        {
            return 0;
        }

        _stats.MAtkSpd = mAtkSpd;

        float stat = StatsConverter.Instance.ConvertStat(Stat.MAGIC_ATTACK_SPEED, mAtkSpd);
        // AnimationController.SetMAtkSpd(stat, 1000);

        return stat;
    }

    public virtual float UpdateRunSpeed(int speed)
    {
        if (speed == _stats.RunSpeed)
        {
            return _stats.ScaledRunSpeed;
        }

        float scaled = StatsConverter.Instance.ConvertStat(Stat.SPEED, speed);
        _stats.RunSpeed = speed;
        _stats.ScaledRunSpeed = scaled;

        AnimationController.SetRunSpeed(scaled);

        return scaled;
    }

    public virtual float UpdateWalkSpeed(int speed)
    {
        if (speed == _stats.WalkSpeed)
        {
            return _stats.ScaledWalkSpeed;
        }

        float scaled = StatsConverter.Instance.ConvertStat(Stat.SPEED, speed);
        _stats.WalkSpeed = speed;
        _stats.ScaledWalkSpeed = scaled;

        AnimationController.SetWalkSpeed(scaled);

        return scaled;
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

    public void CastSkill(Skill skill, Entity target, int hitTime, int reuseDelay, PooledEffect[] castEffects)
    {
        Combat.CastSkill(skill, target, hitTime, reuseDelay, castEffects);
    }

    public void LaunchSkill()
    {
        Combat.ThrowSkill();
    }

    public virtual void UpdateAppearance(Appearance newAppearance)
    {
        Gear.UpdateAppearance(Appearance, newAppearance);
        Appearance = newAppearance;
    }
}
