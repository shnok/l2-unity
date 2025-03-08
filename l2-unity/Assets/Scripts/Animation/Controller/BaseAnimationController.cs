using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAnimationController : MonoBehaviour
{
    [SerializeField] protected Transform _rootBone;
    [SerializeField] protected EntityReferenceHolder _entityReferenceHolder;
    protected Animator Animator { get { return _entityReferenceHolder.Animator; } }
    [SerializeField] protected bool _resetStateOnReceive = false;
    protected float _spAtk01ClipLength = 1000;
    [SerializeField] protected Dictionary<string, float> _atkClipLengths;

    protected SkillCastAnimation _skillCastAnimation;
    protected SkillThrowAnimation _skillThrowAnimation;

    public Transform RootBone { get => _rootBone; }
    private AnimatorParameterHashTable _animatorParameterHashTable;
    public AnimatorParameterHashTable AnimatorParameterHashTable
    {
        get
        {
            _animatorParameterHashTable ??= AnimatorParameterHashTable.Instance;

            return _animatorParameterHashTable;
        }
    }

    protected float _lastAtkClipLength;
    protected float _pAtkSpd;

    public float PAtkSpd { get { return _pAtkSpd; } }

    public virtual void Initialize()
    {
        if (_entityReferenceHolder == null)
        {
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
            _entityReferenceHolder = gameObject.GetComponent<EntityReferenceHolder>();
        }

        if (_rootBone == null)
        {
            Debug.LogWarning($"[{transform.name}] RootBone was not assigned, please pre-assign it to avoid unecessary load.");
            _rootBone = transform.FindRecursive("bip01");
        }
    }

    public virtual void WeaponAnimChanged(WeaponAnimType weapon) { }

    public abstract void SetRunSpeed(float value);

    public abstract void SetWalkSpeed(float value);

    public void SetPAtkSpd(float value)
    {
        _pAtkSpd = value;
        if (_lastAtkClipLength != 0)
        {
            UpdateAnimatorAtkSpdMultiplier(_lastAtkClipLength);
        }
    }

    public abstract void UpdateAnimatorAtkSpdMultiplier(float clipLength);

    public abstract void SetMAtkSpd(float value);

    // Set all animation variables to false
    public void ClearAnimParams()
    {
        for (int i = 0; i < Animator.parameters.Length; i++)
        {
            AnimatorControllerParameter anim = Animator.parameters[i];
            if (anim.type == AnimatorControllerParameterType.Bool)
            {
                Animator.SetBool(anim.name, false);
            }
        }
    }

    public virtual void SetBool(string name, bool value)
    {
        Animator.SetBool(name, value);
    }

    public virtual void SetBool(int parameterId, bool value)
    {
        // Debug.LogWarning($"Set bool {parameterId}={value}");
        Animator.SetBool(parameterId, value);
    }

    public bool GetBool(int parameterId)
    {
        return Animator.GetBool(parameterId);
    }

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

    public virtual bool PlaySkillThrowAnimation()
    {
        return _skillThrowAnimation != SkillThrowAnimation.None;
    }
}
