using System;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

public abstract class NewBaseAnimationController : MonoBehaviour
{
    [SerializeField] protected EntityReferenceHolder _entityReferenceHolder;
    [SerializeField] protected L2Animations _animationClips; //TODO: Cache in Singleton
    protected AnimancerComponent _animancer;
    protected Animator Animator { get { return _entityReferenceHolder.Animator; } }
    protected SkillCastAnimation _skillCastAnimation;
    protected SkillThrowAnimation _skillThrowAnimation;
    [SerializeField] protected Transform _rootBone;
    public Transform RootBone { get => _rootBone; }
    [SerializeField] protected float _fadeDuration = 0.1f;
    [SerializeField] protected float _atkSpd;
    [SerializeField] protected float _atkSpdMultiplier;
    [SerializeField] protected float _castSpdMultiplier;
    [SerializeField] protected float _runSpdMultiplier;
    [SerializeField] protected float _walkSpdMultiplier;
    [SerializeField] protected float _lastPlayedClipDuration;
    protected AnimancerState _animancerState;
    public float PAtkSpd { get => _atkSpd; }

    public virtual void Initialize()
    {
        if (_entityReferenceHolder == null)
        {
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
            _entityReferenceHolder = gameObject.GetComponent<EntityReferenceHolder>();
        }
        if (_animationClips == null)
        {
            Debug.LogWarning($"[{transform.name}] L2Animations was not assigned, please pre-assign it.");
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
        _animancerState = _animancer.Play(_animationClips.AnimationClips[index], _fadeDuration);
    }

    public virtual void WeaponAnimChanged(WeaponAnimType weapon) { }

    public abstract void SetRunSpeed(float value);

    public abstract void SetWalkSpeed(float value);

    public virtual void SetPAtkSpd(float value)
    {
        _atkSpd = value;
        if (_lastPlayedClipDuration != 0)
        {
            UpdateAnimatorAtkSpdMultiplier(_lastPlayedClipDuration, value);
        }
    }

    public abstract void UpdateAnimatorAtkSpdMultiplier(float clipLength, float patkspd);

    public abstract void SetMAtkSpd(float value);
    public abstract void Attack();
    public abstract void Die();
    public abstract void Sit();
    public abstract void SitWait();
    public abstract void Stand();
    public abstract void Run();
    public abstract void Wait();
    public abstract void Walk();

    public virtual void PlaySkillAnimation(SkillCastAnimation castAnimation, SkillThrowAnimation throwAnimation)
    {
        PlaySkillCastAnimation();
    }

    public virtual bool PlaySkillCastAnimation()
    {
        return _skillCastAnimation != SkillCastAnimation.None;
    }

    public virtual bool PlaySkillThrowAnimation()
    {
        return _skillThrowAnimation != SkillThrowAnimation.None;
    }
}
