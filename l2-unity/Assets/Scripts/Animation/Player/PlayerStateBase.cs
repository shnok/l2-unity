using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateBase : StateMachineBehaviour {
    protected PlayerAnimationAudioHandler _audioHandler;
    protected Animator _animator;

    //TODO implement weapon system
    [SerializeField] protected string _weaponType = "hand";

    public void LoadComponents(Animator animator) {
        if (_audioHandler == null) {
            _audioHandler = animator.gameObject.GetComponent<PlayerAnimationAudioHandler>();
        }
        if (this._animator == null) {
            this._animator = animator;
        }
    }

    public void PlaySoundAtRatio(CharacterSoundEvent soundEvent, float ratio) {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void SetBool(string name, bool value) {
        if (_animator.GetBool(name) != value) {
            _animator.SetBool(name, value);
        }
    }
}
