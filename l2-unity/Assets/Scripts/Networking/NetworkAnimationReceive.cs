using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAnimationReceive : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _resetStateOnReceive = false;
    void Start() {
        if(World.Instance.OfflineMode) {
            this.enabled = false;
            return;
        }
        _animator = gameObject.GetComponentInChildren<Animator>(true);
    }

    public void SetFloat(string property, float value) {
        _animator.SetFloat(property, value);
    }

    public void SetAnimationProperty(int animId, float value) {
        if(animId >= 0 && animId < _animator.parameters.Length) {
            if(_resetStateOnReceive) {
                ClearAnimParams();
            }

            AnimatorControllerParameter anim = _animator.parameters[animId];
            //Debug.Log("Updating animation: " + transform.name + " " + anim.name + "=" + value);

            switch(anim.type) {
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
