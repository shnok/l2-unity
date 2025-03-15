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
    protected SkillCastAnimation _skillCastAnimation;
    protected SkillThrowAnimation _skillThrowAnimation;
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

    public abstract void SetMAtkSpd(float value);
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

    // float SHOOT_ARROW_RATIO = 0.6f
    //  private void ManageArrow(AnimatorStateInfo stateInfo)
    // {
    //     float normalizedRatio = stateInfo.normalizedTime - _lastArrowNormalizedTime;

    //     if (normalizedRatio >= 1f)
    //     {
    //         Debug.LogWarning("Reset atk animation state");
    //         _nockedArrow = false;
    //         _shotArrow = false;
    //         _lastArrowNormalizedTime = stateInfo.normalizedTime;
    //     }
    //     else if (normalizedRatio >= SHOOT_ARROW_RATIO)
    //     {
    //         if (!_shotArrow)
    //         {
    //             _shotArrow = true;
    //             _referenceHolder.Combat.ShootArrow();
    //             AudioHandler.PlayArrowShootSound();
    //         }
    //     }
    //     else if (normalizedRatio >= 0.2f)
    //     {
    //         if (!_nockedArrow)
    //         {
    //             _nockedArrow = true;
    //             _referenceHolder.Combat.NockArrow();
    //             AudioHandler.PlayBowBendSound();
    //         }
    //     }
    // }
}
