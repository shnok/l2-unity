using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStateBase : StateMachineBehaviour {
    protected CharacterAnimationAudioHandler _audioHandler;
    protected NetworkCharacterControllerReceive _networkCharacterControllerReceive;
    protected NetworkAnimationController _networkAnimationController;
    protected Animator _animator;
    protected Entity _entity;
    protected UserGear _gear;
    protected bool _cancelAction = false;
    [SerializeField] protected bool _enabled = true;
    [SerializeField] protected string _weaponAnim;
    [SerializeField] protected WeaponType _weaponType;

    public void LoadComponents(Animator animator) {
        if (_entity == null) {
            if(animator.transform.parent == null) {
                _enabled = false;
                return;
            } else if (animator.transform.parent.parent == null) {
                _enabled = false;
                return;
            }
            _entity = animator.transform.parent.parent.GetComponent<Entity>();
        }
        if (_entity == null || _entity is PlayerEntity) {
            _enabled = false;
            return;
        }
        if (_gear == null) {
            _gear = _entity.GetComponent<UserGear>();
        }

        _weaponType = _gear.WeaponType;
        _weaponAnim = _gear.WeaponAnim;

        if (_audioHandler == null) {
            _audioHandler = animator.gameObject.GetComponent<CharacterAnimationAudioHandler>();
        }
        if (_animator == null) {
            _animator = animator;
        }
        if (_networkCharacterControllerReceive == null) {
            _networkCharacterControllerReceive = _entity.transform.GetComponent<NetworkCharacterControllerReceive>();
        }
        if (_networkAnimationController == null) {
            _networkAnimationController = _entity.transform.GetComponent<NetworkAnimationController>();
        }
    }

    public void PlaySoundAtRatio(CharacterSoundEvent soundEvent, float ratio) {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void PlaySoundAtRatio(ItemSoundEvent soundEvent, float ratio) {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void SetBool(string name, bool value) {
        _cancelAction = true;
        //if (value != _animator.GetBool(name)) {
        //    Debug.LogWarning($"Set bool {name}={value}");
        //}
        _animator.SetBool(name, value);
    }
}
