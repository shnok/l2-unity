using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[System.Serializable]
public abstract class Combat : MonoBehaviour
{
    [SerializeField] protected EntityReferenceHolder _referenceHolder;

    [Header("Combat")]
    [SerializeField] private int _targetId;
    [SerializeField] protected Entity _target;
    [SerializeField] protected Entity _attackTarget;
    // [SerializeField] private long _stopAutoAttackTime;
    [SerializeField] private long _combatTimestamp;
    [SerializeField] private float _hitTime;
    [SerializeField] private float _attackEndTime;
    [SerializeField] private bool _hitSuccess;

    public int TargetId { get => _targetId; set => _targetId = value; }
    public Entity Target { get => _target; set => _target = value; }
    public Entity AttackTarget { get => _attackTarget; set => _attackTarget = value; }
    public float AttackEndTime { get => _attackEndTime; }
    public long CombatTimestamp { get => _combatTimestamp; }
    protected Status Status { get => _referenceHolder.Entity.Status; }
    protected BaseAnimationAudioHandler AudioHandler { get => _referenceHolder.AudioHandler; }
    protected BaseAnimationController AnimationController { get => _referenceHolder.AnimationController; }

    private void Awake()
    {
        if (_referenceHolder == null)
        {
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
            _referenceHolder = GetComponent<EntityReferenceHolder>();
        }
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
        _attackEndTime = atkEndTime;

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
        WorldCombat.Instance.EntityShootArrow(_referenceHolder.Entity, AttackTarget, _referenceHolder.Gear.Arrow, _hitTime, _hitSuccess);
        _referenceHolder.Gear.HideArrow();
    }
}
