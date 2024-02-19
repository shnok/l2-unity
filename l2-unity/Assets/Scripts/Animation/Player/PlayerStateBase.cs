using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateBase : StateMachineBehaviour {
    protected CharacterAnimationAudioHandler _audioHandler;
    protected Animator _animator;
    protected PlayerAnimationController _controller;
    protected Entity _entity;

    //TODO implement weapon system
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
        PlayerAnimationController.Instance.SetBool(name, value, true);
    }

    public void SetBool(string name, bool value, bool share) {
        PlayerAnimationController.Instance.SetBool(name, value, share);
    }
}
