using System;
using UnityEngine;

[System.Serializable]
public abstract class Combat : MonoBehaviour
{
    [SerializeField] protected EntityReferenceHolder _referenceHolder;

    [Header("Combat")]
    [SerializeField] private int _targetId;
    [SerializeField] protected Entity _target;
    [SerializeField] protected Entity _attackTarget;
    [SerializeField] private long _combatTimestamp;
    [SerializeField] private float _hitTime;
    [SerializeField] private float _attackEndTime;
    [SerializeField] private bool _hitSuccess;
    [SerializeField] private bool _fightStanceStarted;
    [Header("Skill")]
    [SerializeField] private Skill _lastSkill;
    [SerializeField] private int _lastSkillHitTime;
    [SerializeField] private int _lastSkillReuseDelay;
    [SerializeField] private long _lastSkillUseTime;
    [SerializeField] private PooledEffect[] _castingEffects;
    private long _skillThrowThreshold;
    private long _skillShootThreshold;
    private bool _isFighterSkill;
    [SerializeField] protected Entity _lastSkillTarget;
    [SerializeField] protected bool _castingSkill;
    [SerializeField] private bool _skillThrown; // Throw animation threshold reached?
    [SerializeField] private bool _skillShot; // Projectile threshold reached?

    public int TargetId { get => _targetId; set => _targetId = value; }
    public Entity Target { get => _target; set => _target = value; }
    public Entity AttackTarget { get => _attackTarget; set => _attackTarget = value; }
    // public float AttackEndTime { get => _attackEndTime; }
    public long CombatTimestamp { get => _combatTimestamp; }
    protected Status Status { get => _referenceHolder.Entity.Status; }
    public Skill LastSkill { get => _lastSkill; }
    public int LastSkillHitTime { get => _lastSkillHitTime; }
    public int LastSkillReuseDelay { get => _lastSkillReuseDelay; }
    public long LastSkillUseTime { get => _lastSkillUseTime; }
    public bool IsInFightStance { get => _fightStanceStarted; }


    protected BaseAnimationAudioHandler AudioHandler { get => _referenceHolder.AudioHandler; }

    private void Awake()
    {
        if (_referenceHolder == null)
        {
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
            _referenceHolder = GetComponent<EntityReferenceHolder>();
        }

        _skillThrown = true;
    }

    public virtual void Initialize()
    {
        if (_referenceHolder == null)
        {
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
            _referenceHolder = GetComponent<EntityReferenceHolder>();
        }
    }

    // Called when ApplyDamage packet is received 
    public void ApplyDamage(Hit hit)
    {
        OnHit(hit);

        if (Status.Hp <= 0)
        {
            Debug.LogWarning("Trying to apply damage to a dead entity");
            return;
        }

        Status.Hp = Mathf.Max(Status.Hp - hit.Damage, 0);
    }

    public bool IsDead()
    {
        return Status.IsDead;
    }

    public virtual void OnDeath()
    {
        Status.IsDead = true;
    }

    public virtual void OnRevive()
    {
        Debug.LogWarning("OnRevive: " + transform.name);
        Status.IsDead = false;
    }

    protected virtual void OnHit(Hit hit)
    {
        RefreshCombatTimestamp();

        if (!hit.isMiss())
        {
            AudioHandler.PlayDamageSound();

            if (hit.hasSoulshot())
            {
                if (hit.isCrit())
                {
                    AudioHandler.PlayCritSound();
                }
                AudioHandler.PlaySoulshotSound();
            }
            else if (hit.isCrit())
            {
                AudioHandler.PlayCritSound();
            }
            else
            {
                AudioHandler.PlayDefenseSound();
            }
        }

        // voice_sound_weapon -> play voice based on current weapon equiped (random)
        // defense sound -> only when soulshot is not activated
        // Swish sound -> only when attack missed (attacker)
    }

    public void RefreshCombatTimestamp()
    {
        _combatTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public virtual void OnStopMoving()
    {
    }

    public virtual bool AttackOnce(float hitTime, float atkEndTime, bool hitSuccess, Entity attackTarget)
    {
        if (IsDead())
        {
            return false;
        }

        _hitTime = hitTime;
        _hitSuccess = hitSuccess;
        // _attackEndTime = atkEndTime;

        SetAttackTarget(attackTarget);

        LookAtTarget();

        RefreshCombatTimestamp();

        return true;
    }

    protected virtual void SetAttackTarget(Entity attackTarget)
    {
        _attackTarget = attackTarget;
    }

    protected virtual void LookAtTarget()
    {
        Debug.LogWarning("Wrong call");
    }

    public virtual void NockArrow()
    {
        // Debug.Log($"[{transform.name}] Nock arrow");
        _referenceHolder.Gear.ShowArrow();
        // WorldCombat.Instance.EntityNockArrow(_referenceHolder.Entity);
    }

    public virtual void ShootArrow()
    {
        // Debug.Log($"[{transform.name}] Shoot arrow");
        float timeToReachTarget = WorldCombat.Instance.CalculateTimeToHitTarget(_referenceHolder.Entity, AttackTarget);
        float hitTimeReal = Time.time + timeToReachTarget;

        WorldCombat.Instance.EntityShootArrow(_referenceHolder.Entity, AttackTarget, hitTimeReal, _hitSuccess);
        _referenceHolder.Gear.HideArrow();
    }

    public virtual void CastSkill(Skill skill, Entity target, int hitTime, int reuseDelay, PooledEffect[] castEffects)
    {
        _hitSuccess = true;
        _lastSkill = skill;
        _lastSkillHitTime = hitTime;
        _lastSkillReuseDelay = reuseDelay;
        _lastSkillTarget = target;
        _skillThrown = false;
        _castingSkill = true;
        _skillShot = false;
        _lastSkillUseTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _isFighterSkill = (int)_lastSkill.Skillgrps[0].CastAnimation >= 100;
        _skillThrowThreshold = _lastSkillUseTime + (int)(_lastSkillHitTime * 0.75f);
        _skillShootThreshold = _lastSkillUseTime + (int)(_lastSkillHitTime * 0.90f);
        _castingEffects = castEffects;

        Debug.Log($"CastSkill: skill={skill}, hitTime={hitTime}, reuseDelay={reuseDelay}, _lastSkillUseTime={_lastSkillUseTime}");

        if (skill.Skillgrps[0]?.CastAnimation != SkillCastAnimation.None)
        {
            _referenceHolder.NewAnimationController.PlaySkillAnimation(skill);
            if (_isFighterSkill)
            {
                _referenceHolder.Gear.StartTrail();
            }
        }
    }

    public virtual void AbortCast()
    {
        _castingSkill = false;
        _referenceHolder.Gear.StopTrail();
        _referenceHolder.Gear.HideArrow();

        // Disable effects
        if (_castingEffects != null)
        {
            foreach (PooledEffect effect in _castingEffects)
            {
                effect.GameObject.SetActive(false);
            }

            _castingEffects = null;
        }
    }

    void Update()
    {
        if (_castingSkill)
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            // Debug.Log($"Update: now={now}, endTime={endTime}, timeToThrow={timeToThrow}, difference={endTime - now}");

            // Debug.Log($"Progress: {(endTime - now) / (float)timeToThrow * 100f}%");

            LookAtTarget();

            if (!_skillThrown)  //Play launch animation at 75%
            {
                if (now > _skillThrowThreshold)
                {
                    _skillThrown = true;

                    //Play skill throw animation
                    if (_lastSkill.Skillgrps[0].ThrowAnimation != SkillThrowAnimation.None)
                    {
                        ThrowSkill();
                    }
                }
            }
            else if (!_skillShot) //Throw projectile at 90%
            {
                if (now > _skillShootThreshold)
                {
                    _skillShot = true;

                    //for now ignore the default 20% of cast time as time to hit targetm and use default proj speed
                    // float hitTimeReal = Time.time + (_referenceHolder.Combat.LastSkillHitTime * 0.25f / 1000f);

                    float hitTimeReal = 0;
                    if (_lastSkill.SkillEffect.ShotActions.Count > 0 && !_lastSkill.SkillEffect.ShotActions[0].SpawnOnTarget)
                    {
                        float timeToReachTarget = WorldCombat.Instance.CalculateTimeToHitTarget(_referenceHolder.Entity, _lastSkillTarget);
                        hitTimeReal = Time.time + timeToReachTarget;
                    }

                    WorldCombat.Instance.EntityShootSkill(_referenceHolder.Entity, _lastSkillTarget, _lastSkill, hitTimeReal);
                }
            }
            else if (now > _lastSkillUseTime + _lastSkillHitTime)
            {
                _castingSkill = false;
                _referenceHolder.Gear.StopTrail();
            }
        }
    }

    protected Transform GetTargetToLookAt()
    {
        if (Status.IsDead)
        {
            return null;
        }

        if (_castingSkill && _lastSkillTarget != null)
        {
            return _lastSkillTarget.transform;
        }

        if (AttackTarget != null)
        {
            return AttackTarget.transform;
        }

        return null;
    }

    public virtual void ThrowSkill()
    {
        _referenceHolder.NewAnimationController.PlaySkillThrowAnimation();
    }

    public void FightStanceStart()
    {
        _fightStanceStarted = true;
        _referenceHolder.NewAnimationController.FightStanceStarted();
    }

    public void FightStanceStop()
    {
        _fightStanceStarted = false;
        _referenceHolder.NewAnimationController.FightStanceStopped();
    }
}
