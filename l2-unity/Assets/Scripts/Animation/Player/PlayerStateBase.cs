using UnityEngine;

public class PlayerStateBase : StateMachineBehaviour
{
    protected Animator _animator;
    protected EntityReferenceHolder _referenceHolder;
    protected HumanoidAudioHandler AudioHandler { get { return (HumanoidAudioHandler)_referenceHolder.AudioHandler; } }
    protected PlayerAnimationController AnimationController { get { return (PlayerAnimationController)_referenceHolder.AnimationController; } }
    protected Entity Entity { get { return _referenceHolder.Entity; } }

    public void LoadComponents(Animator animator)
    {
        if (_referenceHolder == null)
        {
            Transform entityTransform = animator.transform.parent.parent;
            _referenceHolder = entityTransform.GetComponent<EntityReferenceHolder>();
        }

        if (_referenceHolder == null)
        {
            Debug.LogError("Reference holder is null");
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
        SetBool(animation, value, true);
    }

    public void SetBool(HumanoidAnimType animation, bool value, bool share)
    {
        // if (isWeaponAnim)
        // {
        //     name += "_" + _gear.WeaponAnim;
        // }

        //Debug.Log("Set bool: " + name);

        PlayerAnimationController.Instance.SetBool(animation, value, share);
    }
}
