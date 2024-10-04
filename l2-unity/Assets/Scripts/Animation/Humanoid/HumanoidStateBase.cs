using System;
using UnityEngine;

public class HumanoidStateBase : StateMachineBehaviour
{
    protected HumanoidAudioHandler _audioHandler;
    protected NetworkCharacterControllerReceive _networkCharacterControllerReceive;
    protected NetworkAnimationController _networkAnimationController;
    protected Entity _entity;
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

        if (_audioHandler == null)
        {
            _audioHandler = animator.gameObject.GetComponent<HumanoidAudioHandler>();
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

    public void SetBool(HumanoidAnimType animation, bool value)
    {
        _cancelAction = true;

        _networkAnimationController.SetBool(animation, value);
    }

    public bool GetBool(HumanoidAnimType animation)
    {
        return _networkAnimationController.GetBool(animation);
    }
}
