using System;
using UnityEngine;

public class HumanoidStateBase : StateMachineBehaviour
{
    protected HumanoidAudioHandler _audioHandler;
    protected NetworkCharacterControllerReceive _networkCharacterControllerReceive;
    protected NetworkAnimationController _networkAnimationController;
    protected Animator _animator;
    protected Entity _entity;
    protected Gear _gear;
    protected bool _cancelAction = false;

    public void LoadComponents(Animator animator)
    {
        Transform entityTransform = animator.transform;
        if (_entity == null)
        {
            _entity = entityTransform.GetComponent<Entity>();

            if (_entity == null)
            {
                entityTransform = animator.transform.parent.parent;
                _entity = entityTransform.GetComponent<Entity>();
            }
        }

        if (_gear == null)
        {
            _gear = entityTransform.GetComponent<Gear>();
        }

        if (_audioHandler == null)
        {
            _audioHandler = animator.gameObject.GetComponent<HumanoidAudioHandler>();
        }
        if (_animator == null)
        {
            _animator = animator;
        }
        if (_networkCharacterControllerReceive == null)
        {
            _networkCharacterControllerReceive = entityTransform.GetComponent<NetworkCharacterControllerReceive>();
        }
        if (_networkAnimationController == null)
        {
            _networkAnimationController = entityTransform.GetComponent<NetworkAnimationController>();
        }
    }

    public void PlaySoundAtRatio(EntitySoundEvent soundEvent, float ratio)
    {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void PlaySoundAtRatio(ItemSoundEvent soundEvent, float ratio)
    {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void SetBool(string name, bool isWeaponAnim, bool value)
    {
        _cancelAction = true;

        if (isWeaponAnim)
        {
            name += "_" + _gear.WeaponAnim;
        }
        //if (value != _animator.GetBool(name)) {
        //    Debug.LogWarning($"Set bool {name}={value}");
        //}

        _networkAnimationController.SetBool(name, value);
    }
}
