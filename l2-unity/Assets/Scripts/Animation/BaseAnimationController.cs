using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimationController : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected bool _resetStateOnReceive = false;
    [SerializeField] protected float _atk01ClipLength = 1000;
    [SerializeField] protected float _spAtk01ClipLength = 1000;

    public virtual void Initialize() {
        _animator = gameObject.GetComponentInChildren<Animator>(true);
        FetchAnimationClipLengths();
    }

    private void FetchAnimationClipLengths() {
        RuntimeAnimatorController rac = _animator.runtimeAnimatorController;
        AnimationClip[] clips = rac.animationClips;
        bool foundSpAtk = false;
        for (int i = 0; i < clips.Length; i++) {
            if (clips[i] == null) {
                continue;
            }

            if (clips[i].name.ToLower().Contains("atk01")) {
                _atk01ClipLength = clips[i].length * 1000;
            }

            if (clips[i].name.ToLower().Contains("spatk01")) {
                foundSpAtk = true;
                _spAtk01ClipLength = clips[i].length * 1000;
            }

            if (clips[i].name.ToLower().Contains("spwait01")) {
                if (!foundSpAtk) {
                    _spAtk01ClipLength = clips[i].length * 1000;
                }
            }
        }
    }

    public void SetMoveSpeed(float value) {
        _animator.SetFloat("speed", value);
    }

    public void SetPAtkSpd(float value) {
        float newAtkSpd = _atk01ClipLength / value;
        _animator.SetFloat("patkspd", newAtkSpd);
    }

    public void SetMAtkSpd(float value) {
        //TODO: update for cast animation
        float newMAtkSpd = _spAtk01ClipLength / value;
        _animator.SetFloat("matkspd", newMAtkSpd);
    }

    public void ClearAnimParams() {
        for (int i = 0; i < _animator.parameters.Length; i++) {
            AnimatorControllerParameter anim = _animator.parameters[i];
            if (anim.type == AnimatorControllerParameterType.Bool) {
                _animator.SetBool(anim.name, false);
            }
        }
    }

    public void SetBool(string name, bool value) {
        _animator.SetBool(name, value);
    }

    public void SetAnimationProperty(int animId, float value) {
        SetAnimationProperty(animId, value, false);
    }

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
                    _animator.SetBool(anim.name, value == 1f);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    _animator.SetTrigger(anim.name);
                    break;
            }
        }
    }

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
