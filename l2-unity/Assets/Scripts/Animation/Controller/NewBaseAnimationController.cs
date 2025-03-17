using System;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

public abstract class NewBaseAnimationController : MonoBehaviour
{
    [SerializeField] protected EntityReferenceHolder _entityReferenceHolder;
    protected AnimancerComponent _animancer;
    [SerializeField] protected int _lastAnim;
    protected Animator Animator { get { return _entityReferenceHolder.Animator; } }
    [SerializeField] protected SkillCastAnimation _skillCastAnimation;
    [SerializeField] protected SkillThrowAnimation _skillThrowAnimation;
    [SerializeField] protected Transform _rootBone;
    public Transform RootBone { get => _rootBone; }
    [SerializeField] protected float _lastPlayedClipDuration;
    [SerializeField] protected float _fadeDuration = 0.1f;
    [Header("Speed Multipliers")]
    [SerializeField] protected float _atkSpd;
    [SerializeField] protected float _atkSpdMultiplier = 1;
    [SerializeField] protected float _castSpdMultiplier = 1;
    [SerializeField] protected float _runSpdMultiplier = 1;
    [SerializeField] protected float _walkSpdMultiplier = 1;
    [Header("Bow")]
    [SerializeField] protected float _nockArrowRatio = 0.2f;
    [SerializeField] protected float _shootArrowRatio = 0.6f;

    protected AnimancerState _animancerState;
    public float PAtkSpd { get => _atkSpd; }

    public virtual void Initialize()
    {
        if (_entityReferenceHolder == null)
        {
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
            _entityReferenceHolder = gameObject.GetComponent<EntityReferenceHolder>();
        }
        if (_animancer == null)
        {
            _animancer = _entityReferenceHolder.Animancer;
        }
        if (_rootBone == null)
        {
            Debug.LogWarning($"[{transform.name}] RootBone was not assigned, please pre-assign it to avoid unecessary load.");
            _rootBone = transform.FindRecursive("bip01");
        }
    }

    public virtual void PlayAnimation(int index)
    {
        _lastAnim = index;

        AnimationClip clip = GetAnimationClip(index);
        if (clip == null)
        {
            Debug.LogWarning($"[{transform.name}] Does not have an animation clip at index {index}.");
            return;
        }

        _lastPlayedClipDuration = clip.length;

        Debug.LogWarning(index);
        Debug.LogWarning(clip);
        Debug.LogWarning(_animancer);
        _animancerState = _animancer.Play(clip, _fadeDuration);
    }

    protected abstract AnimationClip GetAnimationClip(int index);

    public virtual void WeaponAnimChanged(WeaponAnimType weapon) { }

    public virtual void SetRunSpeed(float value)
    {
        _runSpdMultiplier = value;
    }

    public virtual void SetWalkSpeed(float value)
    {
        _walkSpdMultiplier = value;
    }

    public virtual void SetPAtkSpd(float value)
    {
        _atkSpd = value;
        UpdateAttackAnimationSpeed(_lastPlayedClipDuration, value);
    }

    public virtual void UpdateAttackAnimationSpeed(float clipLength, float patkspd)
    {
        float newAtkSpd = clipLength * 1000f / patkspd;
        _atkSpdMultiplier = newAtkSpd;
    }

    public void UpdateCastAnimationSpeed(float clipLength, float skillHitTime)
    {
        float clipLengthMs = clipLength * 1000f;
        float skillCastEndTime = skillHitTime / 2f; // at 50% of cast time should switch to castend anim
        float castSpeed = clipLengthMs / skillCastEndTime;
        _castSpdMultiplier = castSpeed;
    }

    public abstract void Attack();
    public abstract void Die();
    public abstract void DieWait();
    public abstract void Resurrect();
    public abstract void Sit();
    public abstract void SitWait();
    public abstract void Stand();
    public abstract void Run();
    public abstract void Wait();
    public abstract void Walk();
    public abstract void AtkWait();

    public virtual void PlaySkillAnimation(SkillCastAnimation castAnimation, SkillThrowAnimation throwAnimation)
    {
        _skillCastAnimation = castAnimation;
        _skillThrowAnimation = throwAnimation;

        PlaySkillCastAnimation();
    }

    public virtual bool PlaySkillCastAnimation()
    {
        return _skillCastAnimation != SkillCastAnimation.None;
    }

    public virtual void PlaySkillCastEndAnimation() { }

    public virtual bool PlaySkillThrowAnimation()
    {
        return _skillThrowAnimation != SkillThrowAnimation.None;
    }

    public virtual void Move()
    {
        if (_entityReferenceHolder.Entity.Running)
        {
            Run();
        }
        else
        {
            Walk();
        }
    }
}
