using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAnimationReceive : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _resetStateOnReceive = false;
    [SerializeField] private float _atk01ClipLength = 1000;
    [SerializeField] private float _spAtk01ClipLength = 1000;

    void Awake() {
        if(World.Instance.OfflineMode) {
            this.enabled = false;
            return;
        }
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

            if(clips[i].name.ToLower().Contains("atk01")) {
                _atk01ClipLength = clips[i].length * 1000;
            }

            if (clips[i].name.ToLower().Contains("spatk01")) {
                foundSpAtk = true;
                _spAtk01ClipLength = clips[i].length * 1000;
            }

            if (clips[i].name.ToLower().Contains("spwait01")) {
                if(!foundSpAtk) {
                    _spAtk01ClipLength = clips[i].length * 1000;
                }
            }
        }
    }

    public void SetFloat(string property, float value) {
        _animator.SetFloat(property, value);
    }

    public void SetAnimationProperty(int animId, float value) {
        SetAnimationProperty(animId, value, false);
    }

    public void SetMoveSpeed(float value) {
        _animator.SetFloat("speed", value);
    }

    public void SetPAtkSpd(float value) {
        float newAtkSpd = _atk01ClipLength / value;
        SetFloat("patkspd", newAtkSpd);
    }

    public void SetMAtkSpd(float value) {
        //TODO: update for cast animation
        float newMAtkSpd = _spAtk01ClipLength / value;
        SetFloat("matkspd", newMAtkSpd);
    }

    public void SetAnimationProperty(int animId, float value, bool forceReset) {
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
                    _animator.SetBool(anim.name, (int)value == 1);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    _animator.SetTrigger(anim.name);
                    break;
            }
        }
    }

    public void ClearAnimParams() {
        for(int i = 0; i < _animator.parameters.Length; i++) {
            AnimatorControllerParameter anim = _animator.parameters[i];
            if(anim.type == AnimatorControllerParameterType.Bool) {
                _animator.SetBool(anim.name, false);
            }
        }
    }
}
