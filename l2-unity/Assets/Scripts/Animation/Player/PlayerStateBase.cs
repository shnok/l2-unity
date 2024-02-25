using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateBase : StateMachineBehaviour {
    protected CharacterAnimationAudioHandler _audioHandler;
    protected Animator _animator;
    protected PlayerAnimationController _controller;
    protected Entity _entity;
    [SerializeField] protected bool _enabled = true;
    [SerializeField] protected WeaponType _weaponType;
    [SerializeField] protected string _weaponAnim;

    public void LoadComponents(Animator animator) {
        if (_entity == null) {
            _entity = animator.transform.parent.GetComponent<Entity>();
        }
        if (_entity == null || _entity is UserEntity) {
            _enabled = false;
            return;
        }

        _weaponType = _entity.WeaponType;
        _weaponAnim = _entity.WeaponAnim;

        if (_audioHandler == null) {
            _audioHandler = animator.gameObject.GetComponent<CharacterAnimationAudioHandler>();
        }
        if (_animator == null) {
            _animator = animator;
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
