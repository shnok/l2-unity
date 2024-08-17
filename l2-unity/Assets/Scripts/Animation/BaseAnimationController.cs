using System.Collections.Generic;
using UnityEngine;

public class BaseAnimationController : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected bool _resetStateOnReceive = false;
    [SerializeField] protected float _spAtk01ClipLength = 1000;
    [SerializeField] protected Dictionary<string, float> _atkClipLengths;

    private string _lastAnimationVariableName;
    private float _lastAtkClipLength;
    private float _pAtkSpd;

    public virtual void Initialize() {
        _animator = gameObject.GetComponentInChildren<Animator>(true);
        _lastAnimationVariableName = "wait_hand";
    }

    public void SetMoveSpeed(float value) {
        _animator.SetFloat("speed", value);
    }

    public void SetPAtkSpd(float value) {
        _pAtkSpd = value;
        if (_lastAtkClipLength != 0) {
            UpdateAnimatorAtkSpdMultiplier(_lastAtkClipLength);
        }
    }

    public void UpdateAnimatorAtkSpdMultiplier(float clipLength) {
        float newAtkSpd = clipLength * 1000f / _pAtkSpd;
        _animator.SetFloat("patkspd", newAtkSpd);
    }

    public void SetMAtkSpd(float value) {
        //TODO: update for cast animation
        float newMAtkSpd = _spAtk01ClipLength / value;
        _animator.SetFloat("matkspd", newMAtkSpd);
    }

    // Set all animation variables to false
    public void ClearAnimParams() {
        for (int i = 0; i < _animator.parameters.Length; i++) {
            AnimatorControllerParameter anim = _animator.parameters[i];
            if (anim.type == AnimatorControllerParameterType.Bool) {
                _animator.SetBool(anim.name, false);
            }
        }
    }

    public void WeaponAnimChanged(string newWeaponAnim) {
        ClearAnimParams();

        if(!_lastAnimationVariableName.Contains("_")) {
            Debug.LogWarning($"The last animation was not a weapon animation: {_lastAnimationVariableName}");
            // The last animation was not a weapon animation
            return;
        }

        string[] parts = _lastAnimationVariableName.Split("_");
        if(parts.Length < 1) {
            // Should not happen
            Debug.LogWarning($"Error while parsing previous animation name: {_lastAnimationVariableName}");
            return;
        }

        string newAnimation = parts[0] + "_" + newWeaponAnim;
        Debug.LogWarning($"New animation name: {newAnimation}");
        SetBool(newAnimation, true);
    }

    public void SetBool(string name, bool value) {
        //if (value != _animator.GetBool(name)) {
        //    Debug.LogWarning($"Set bool {name}={value}");
        //}

        // Save the last animation name
        if(value == true) {
            _lastAnimationVariableName = name;
        }

        _animator.SetBool(name, value);
    }

    // Update animator variable based on Animation Id
    public void SetAnimationProperty(int animId, float value) {
        SetAnimationProperty(animId, value, false);
    }

    // Update animator variable based on Animation Id
    public void SetAnimationProperty(int animId, float value, bool forceReset) {
        //Debug.Log("animId " + animId + "/" + _animator.parameters.Length);
        if (animId >= 0 && animId < _animator.parameters.Length) {
            if (_resetStateOnReceive || forceReset) {
                ClearAnimParams();
            }

            AnimatorControllerParameter anim = _animator.parameters[animId];

            switch (anim.type) {
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
    public float GetAnimationProperty(int animId) {
        if (animId >= 0 && animId < _animator.parameters.Length) {   
            AnimatorControllerParameter anim = _animator.parameters[animId];

            switch (anim.type) {
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
}
