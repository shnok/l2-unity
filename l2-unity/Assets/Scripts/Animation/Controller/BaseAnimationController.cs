using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAnimationController : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
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
        if (_animator == null)
        {
            Debug.LogWarning($"[{transform.name}] Animator was not assigned, please pre-assign it to avoid unecessary load.");
            _animator = gameObject.GetComponentInChildren<Animator>(true);
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
        for (int i = 0; i < _animator.parameters.Length; i++)
        {
            AnimatorControllerParameter anim = _animator.parameters[i];
            if (anim.type == AnimatorControllerParameterType.Bool)
            {
                _animator.SetBool(anim.name, false);
            }
        }
    }

    public virtual void SetBool(string name, bool value)
    {
        _animator.SetBool(name, value);
    }

    public virtual void SetBool(int parameterId, bool value)
    {
        _animator.SetBool(parameterId, value);
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
        if (parameterId >= 0 && parameterId < _animator.parameters.Length)
        {
            if (_resetStateOnReceive || forceReset)
            {
                ClearAnimParams();
            }

            Debug.Log("Obsolete: Please move away from straight animation id sharing.");
            AnimatorControllerParameter anim = _animator.parameters[parameterId];

            switch (anim.type)
            {
                case AnimatorControllerParameterType.Float:
                    _animator.SetFloat(anim.name, value);
                    break;
                case AnimatorControllerParameterType.Int:
                    _animator.SetInteger(anim.name, (int)value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    SetBool(anim.name, value == 1f);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    _animator.SetTrigger(anim.name);
                    break;
            }
        }
    }

    // Return an animator variable based on its ID
    public float GetAnimationProperty(int animId)
    {
        if (animId >= 0 && animId < _animator.parameters.Length)
        {
            AnimatorControllerParameter anim = _animator.parameters[animId];

            switch (anim.type)
            {
                case AnimatorControllerParameterType.Float:
                    return _animator.GetFloat(anim.name);
                case AnimatorControllerParameterType.Int:
                    return (int)_animator.GetFloat(anim.name);
                case AnimatorControllerParameterType.Bool:
                    return _animator.GetBool(anim.name) == true ? 1f : 0;
            }
        }

        return 0f;
    }

    public bool GetBool(int parameterId)
    {
        return _animator.GetBool(parameterId);
    }
}
