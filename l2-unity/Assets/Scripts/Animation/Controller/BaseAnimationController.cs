using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAnimationController : MonoBehaviour
{
    [SerializeField] protected EntityReferenceHolder _entityReferenceHolder;
    protected Animator Animator { get { return _entityReferenceHolder.Animator; } }
    [SerializeField] protected bool _resetStateOnReceive = false;
    protected float _spAtk01ClipLength = 1000;
    [SerializeField] protected Dictionary<string, float> _atkClipLengths;

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

    public virtual void Initialize()
    {
        if (_entityReferenceHolder == null)
        {
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
            _entityReferenceHolder = gameObject.GetComponent<EntityReferenceHolder>();
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
        Animator.SetBool(parameterId, value);
    }

    // Update animator variable based on Animation Id
    public void SetAnimationProperty(int animId, float value)
    {
        SetAnimationProperty(animId, value, false);
    }

    // Update animator variable based on Animation Id
    public void SetAnimationProperty(int parameterId, float value, bool forceReset)
    {
        //Debug.Log("animId " + animId + "/" + _animator.parameters.Length);
        if (parameterId >= 0 && parameterId < Animator.parameters.Length)
        {
            if (_resetStateOnReceive || forceReset)
            {
                ClearAnimParams();
            }

            Debug.Log("Obsolete: Please move away from straight animation id sharing.");
            AnimatorControllerParameter anim = Animator.parameters[parameterId];

            switch (anim.type)
            {
                case AnimatorControllerParameterType.Float:
                    Animator.SetFloat(anim.name, value);
                    break;
                case AnimatorControllerParameterType.Int:
                    Animator.SetInteger(anim.name, (int)value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    SetBool(anim.name, value == 1f);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    Animator.SetTrigger(anim.name);
                    break;
            }
        }
    }

    // Return an animator variable based on its ID
    public float GetAnimationProperty(int animId)
    {
        if (animId >= 0 && animId < Animator.parameters.Length)
        {
            AnimatorControllerParameter anim = Animator.parameters[animId];

            switch (anim.type)
            {
                case AnimatorControllerParameterType.Float:
                    return Animator.GetFloat(anim.name);
                case AnimatorControllerParameterType.Int:
                    return (int)Animator.GetFloat(anim.name);
                case AnimatorControllerParameterType.Bool:
                    return Animator.GetBool(anim.name) == true ? 1f : 0;
            }
        }

        return 0f;
    }

    public bool GetBool(int parameterId)
    {
        return Animator.GetBool(parameterId);
    }
}
