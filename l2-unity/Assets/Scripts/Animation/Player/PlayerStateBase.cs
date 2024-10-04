using UnityEngine;

public class PlayerStateBase : StateMachineBehaviour
{
    protected HumanoidAudioHandler _audioHandler;
    protected Animator _animator;
    protected Entity _entity;
    // protected UserGear _gear;

    public void LoadComponents(Animator animator)
    {
        if (_entity == null)
        {
            _entity = animator.transform.parent.parent.GetComponent<Entity>();
        }
        // if (_gear == null)
        // {
        //     _gear = _entity.GetComponent<UserGear>();
        // }
        if (_audioHandler == null)
        {
            _audioHandler = animator.gameObject.GetComponent<HumanoidAudioHandler>();
        }
        if (_animator == null)
        {
            _animator = animator;
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
