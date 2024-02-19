using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateBase : StateMachineBehaviour {
    protected CharacterAnimationAudioHandler _audioHandler;
    protected Animator _animator;
    protected PlayerAnimationController _controller;
    protected Entity _entity;

    [SerializeField] protected WeaponType _weaponType;

    public void LoadComponents(Animator animator) {
        if (_audioHandler == null) {
            _audioHandler = animator.gameObject.GetComponent<CharacterAnimationAudioHandler>();
        }
        if (_animator == null) {
            _animator = animator;
        }
        if (_entity == null) {
            _entity = PlayerEntity.Instance;
        }
        if (_entity != null) {
            _weaponType = _entity.WeaponType;
        }
    }

    public void PlaySoundAtRatio(CharacterSoundEvent soundEvent, float ratio) {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void PlaySoundAtRatio(ItemSoundEvent soundEvent, float ratio) {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void SetBool(string name, bool value) {
        _animator.SetBool(name, value);
    }
}
