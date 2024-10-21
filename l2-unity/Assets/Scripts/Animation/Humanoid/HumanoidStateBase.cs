using System;
using UnityEngine;

public class HumanoidStateBase : StateMachineBehaviour
{
    protected NetworkEntityReferenceHolder _referenceHolder;

    protected HumanoidAudioHandler AudioHandler { get { return (HumanoidAudioHandler)_referenceHolder.AudioHandler; } }
    protected NetworkCharacterControllerReceive CharacterController { get { return (NetworkCharacterControllerReceive)_referenceHolder.NetworkCharacterControllerReceive; } }
    protected NetworkAnimationController AnimationController { get { return (NetworkAnimationController)_referenceHolder.AnimationController; } }
    protected Entity Entity { get { return _referenceHolder.Entity; } }

    protected bool _cancelAction = false;

    public void LoadComponents(Animator animator)
    {
        if (_referenceHolder == null)
        {
            Transform entityTransform = animator.transform;
            _referenceHolder = entityTransform.GetComponent<NetworkEntityReferenceHolder>();

            if (_referenceHolder == null)
            {
                entityTransform = animator.transform.parent.parent;
                _referenceHolder = entityTransform.GetComponent<NetworkEntityReferenceHolder>();
            }
        }
    }

    public void PlayAtkSoundAtRatio(float ratio)
    {
        AudioHandler.PlayAtkSoundAtRatio(ratio);
    }

    public void PlaySoundAtRatio(EntitySoundEvent soundEvent, float ratio)
    {
        AudioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void PlaySoundAtRatio(ItemSoundEvent soundEvent, float ratio)
    {
        AudioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void SetBool(HumanoidAnimType animation, bool value)
    {
        _cancelAction = true;

        AnimationController.SetBool(animation, value);
    }

    public bool GetBool(HumanoidAnimType animation)
    {
        return AnimationController.GetBool(animation);
    }
}
