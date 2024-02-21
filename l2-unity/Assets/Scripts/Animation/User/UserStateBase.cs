using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateBase : StateMachineBehaviour {
    protected CharacterAnimationAudioHandler _audioHandler;
    protected NetworkCharacterControllerReceive _networkCharacterControllerReceive;
    protected Animator _animator;
    protected PlayerAnimationController _controller;
    protected Entity _entity;
    [SerializeField] protected bool _enabled = true;

    [SerializeField] protected WeaponType _weaponType;

    public void LoadComponents(Animator animator) {
        if (_entity == null) {
            _entity = animator.transform.parent.parent.GetComponent<Entity>();
        }
        if (_entity == null || _entity is PlayerEntity) {
            _enabled = false;
            return;
        }

        _weaponType = _entity.WeaponType;


        if (_audioHandler == null) {
            _audioHandler = animator.gameObject.GetComponent<CharacterAnimationAudioHandler>();
        }
        if (_animator == null) {
            _animator = animator;
        }
        if (_networkCharacterControllerReceive == null) {
            _networkCharacterControllerReceive = _entity.transform.GetComponent<NetworkCharacterControllerReceive>();
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
