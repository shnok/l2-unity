using UnityEngine;

public class PlayerStateBase : StateMachineBehaviour {
    protected CharacterAnimationAudioHandler _audioHandler;
    protected Animator _animator;
    protected Entity _entity;
    protected PlayerGear _gear;
    [SerializeField] protected bool _enabled = true;

    public void LoadComponents(Animator animator) {
        if (_entity == null) {
            _entity = animator.transform.parent.GetComponent<Entity>();
        }
        if (_entity == null || _entity is UserEntity) {
            _enabled = false;
            return;
        }

        if (_gear == null) {
            _gear = _entity.GetComponent<PlayerGear>();
        }

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

    public void SetBool(string name, bool isWeaponAnim, bool value) {
        SetBool(name, isWeaponAnim, value, true);
    }

    public void SetBool(string name, bool isWeaponAnim, bool value, bool share) {
        if(isWeaponAnim) {
            name += "_" + _gear.WeaponAnim;
        }

        //Debug.Log("Set bool: " + name);

        PlayerAnimationController.Instance.SetBool(name, value, share);
    }
}
