using UnityEngine;

public class MonsterStateBase : StateMachineBehaviour
{
    protected NetworkEntityReferenceHolder _referenceHolder;

    protected MonsterAudioHandler AudioHandler { get { return (MonsterAudioHandler)_referenceHolder.AudioHandler; } }
    protected NetworkCharacterControllerReceive CharacterController { get { return (NetworkCharacterControllerReceive)_referenceHolder.NetworkCharacterControllerReceive; } }
    protected MonsterAnimationController AnimController { get { return (MonsterAnimationController)_referenceHolder.AnimationController; } }
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

    public void PlaySoundAtRatio(EntitySoundEvent soundEvent, float ratio)
    {
        AudioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void SetBool(MonsterAnimationEvent animationEvent, bool value)
    {
        AnimController.SetBool(animationEvent, value);
    }
}
