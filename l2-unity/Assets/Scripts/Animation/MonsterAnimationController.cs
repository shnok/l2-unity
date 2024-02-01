using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(MonsterAnimationAudioHandler))]
public class MonsterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _npcTemplateName;
    [SerializeField] private string _animationClipName;
    private bool animationStarting = false;

    private void Awake() {
        if(_animator == null) {
            _animator = GetComponent<Animator>();
        }

        if(_animator.runtimeAnimatorController == null) {
            if(_npcTemplateName.Length == 0) {
                _npcTemplateName = transform.name;
            }
            _animator.runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Data/Animations/Animator/" + _npcTemplateName);
        }
    }

    private bool IsAnimationStarting() {
        bool initialState = animationStarting;
        animationStarting = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.05f;
        if(initialState == false && animationStarting == true) {
            return true;
        }
        return false;
    }

    private bool IsCurrentState(string state) {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    private bool IsAnimationFinished() {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f;
    }

    private bool IsNextState(string state) {
        return _animator.GetNextAnimatorStateInfo(0).IsName(state);
    }

    void SetBool(string name, bool value) {
        if(_animator.GetBool(name) != value) {
            _animator.SetBool(name, value);
        }
    }
}
