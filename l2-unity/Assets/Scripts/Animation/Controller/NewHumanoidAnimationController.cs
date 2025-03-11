using System.Collections.Generic;
using Animancer;
using UnityEngine;

// Used by NPCS and USERS
public class NewHumanoidAnimationController : NewBaseAnimationController
{

    public WeaponAnimType WeaponAnim { get { return _weaponAnim; } }
    public HumanoidAnimType LastAnim { get { return _lastAnimationType; } }
    private HumanoidAudioHandler AudioHandler { get => (HumanoidAudioHandler)_entityReferenceHolder.AudioHandler; }

    [Header("Base Speeds")]
    [SerializeField] private float _defaultIdleAnimationSpeed = 0.3f;
    [SerializeField] private float _defaultAtkWaitAnimationSpeed = 0.5f;
    [SerializeField] private float _defaultRunAnimationSpeed = 0.35f;
    [SerializeField] private float _defaultWalkAnimationSpeed = 0.4f;
    [SerializeField] private float _defaultJumpAnimationSpeed = 1.25f;
    [SerializeField] private float _defaultDieAnimationSpeed = 0.5f;

    [Header("Humanoids")]
    [SerializeField] protected HumanoidAnimType _lastAnimationType;
    [SerializeField] protected WeaponAnimType _weaponAnim;
    [SerializeField] private int _atkAnimIndex;

    public override void Initialize()
    {
        base.Initialize();
        _lastAnimationType = HumanoidAnimType.wait;
    }

    public override void WeaponAnimChanged(WeaponAnimType newWeaponAnim)
    {
        _weaponAnim = newWeaponAnim;

        if (!((int)_lastAnimationType != (int)HumanoidAnimType.other))
        {
            Debug.LogWarning($"The last animation was not a weapon animation: {_lastAnimationType}");
            // The last animation was not a weapon animation
            return;
        }

        // Adding weaponanim index to humanoidanimtype will give the correct animationEvent
        HumanoidAnimationEvent newAnim = (HumanoidAnimationEvent)(int)_lastAnimationType + (int)_weaponAnim;

        Debug.Log($"New Weapon animation: {newAnim} Last animation type: {_lastAnimationType} Weapon anim: {_weaponAnim}");

        PlayAnimation((int)newAnim);
    }


    public override void SetMAtkSpd(float clipLength)
    {
        float castSpeed = clipLength * 1000f / (_entityReferenceHolder.Combat.LastSkillHitTime / 2f); // at 50% of cast time should switch to castend anim
        _castSpdMultiplier = castSpeed;
    }

    public override void SetRunSpeed(float value)
    {
        _runSpdMultiplier = value;
    }

    public override void SetWalkSpeed(float value)
    {
        _walkSpdMultiplier = value;
    }

    public override void Attack()
    {
        _lastAnimationType = HumanoidAnimType.atk01;
        _atkAnimIndex = 0;
        PlayAttackAnimation();
    }

    private void NextAttack()
    {
        _atkAnimIndex += 1;

        //1HS has 3 anims
        //2HS has 3 anims
        //Pole has 3 anims
        //Hands has 1 anims
        //Duals has 2 anims
        int maxAttackAnimIndex = 0;
        switch (_weaponAnim)
        {
            case WeaponAnimType._2HS:
            case WeaponAnimType.pole:
            case WeaponAnimType._1HS:
                maxAttackAnimIndex = 2;
                break;
            case WeaponAnimType.dual:
                maxAttackAnimIndex = 1;
                break;
            case WeaponAnimType.bow:
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
            default:
                break;
        }

        if (_atkAnimIndex > maxAttackAnimIndex)
        {
            _atkAnimIndex = 0;
        }

        PlayAttackAnimation();
    }

    private void PlayAttackAnimation()
    {
        HumanoidAnimationEvent toPlay;
        switch (_weaponAnim)
        {
            case WeaponAnimType._1HS:
                toPlay = HumanoidAnimationEvent.atk01_1HS;
                break;
            case WeaponAnimType._2HS:
                toPlay = HumanoidAnimationEvent.atk01_2HS;
                break;
            case WeaponAnimType.bow:
                toPlay = HumanoidAnimationEvent.atk01_bow;
                break;
            case WeaponAnimType.pole:
                toPlay = HumanoidAnimationEvent.atk01_pole;
                break;
            case WeaponAnimType.dual:
                toPlay = HumanoidAnimationEvent.atk01_dual;
                break;
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
            default:
                toPlay = HumanoidAnimationEvent.atk01_hand;
                break;
        }

        PlayAnimation((int)toPlay + _atkAnimIndex);
        UpdateAttackAnimationSpeed(_lastPlayedClipDuration, _atkSpd);

        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {
            events.Add(AudioHandler.AtkRatio, () => AudioHandler.PlayAtkSound());
            events.OnEnd = NextAttack;
        }

        _animancerState.EffectiveSpeed = _atkSpdMultiplier;
    }

    public override void Run()
    {
        _lastAnimationType = HumanoidAnimType.run;
        switch (_weaponAnim)
        {
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
                PlayAnimation((int)HumanoidAnimationEvent.run_hand);
                break;
            case WeaponAnimType._1HS:
                PlayAnimation((int)HumanoidAnimationEvent.run_1HS);
                break;
            case WeaponAnimType._2HS:
                PlayAnimation((int)HumanoidAnimationEvent.run_2HS);
                break;
            case WeaponAnimType.bow:
                PlayAnimation((int)HumanoidAnimationEvent.run_bow);
                break;
            case WeaponAnimType.pole:
                PlayAnimation((int)HumanoidAnimationEvent.run_pole);
                break;
            case WeaponAnimType.dual:
                PlayAnimation((int)HumanoidAnimationEvent.run_dual);
                break;
        }

        _animancerState.EffectiveSpeed = _runSpdMultiplier * _defaultRunAnimationSpeed;
        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {
            foreach (float ratio in AudioHandler.RunStepRatios)
            {
                events.Add(0.5f, () => AudioHandler.PlayBreatheSound());
                events.Add(ratio, () => AudioHandler.PlaySound(EntitySoundEvent.Step));
            }
        }
    }

    public override void Wait()
    {
        _lastAnimationType = HumanoidAnimType.wait;
        switch (_weaponAnim)
        {
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
                PlayAnimation((int)HumanoidAnimationEvent.wait_hand);
                break;
            case WeaponAnimType._1HS:
                PlayAnimation((int)HumanoidAnimationEvent.wait_1HS);
                break;
            case WeaponAnimType._2HS:
                PlayAnimation((int)HumanoidAnimationEvent.wait_2HS);
                break;
            case WeaponAnimType.bow:
                PlayAnimation((int)HumanoidAnimationEvent.wait_bow);
                break;
            case WeaponAnimType.pole:
                PlayAnimation((int)HumanoidAnimationEvent.wait_pole);
                break;
            case WeaponAnimType.dual:
                PlayAnimation((int)HumanoidAnimationEvent.wait_dual);
                break;
        }

        _animancerState.EffectiveSpeed = _defaultIdleAnimationSpeed;

    }

    public override void Walk()
    {
        _lastAnimationType = HumanoidAnimType.walk;
        switch (_weaponAnim)
        {
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
                PlayAnimation((int)HumanoidAnimationEvent.walk_hand);
                break;
            case WeaponAnimType._1HS:
                PlayAnimation((int)HumanoidAnimationEvent.walk_1HS);
                break;
            case WeaponAnimType._2HS:
                PlayAnimation((int)HumanoidAnimationEvent.walk_2HS);
                break;
            case WeaponAnimType.bow:
                PlayAnimation((int)HumanoidAnimationEvent.walk_bow);
                break;
            case WeaponAnimType.pole:
                PlayAnimation((int)HumanoidAnimationEvent.walk_pole);
                break;
            case WeaponAnimType.dual:
                PlayAnimation((int)HumanoidAnimationEvent.walk_dual);
                break;
        }

        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {
            foreach (float ratio in AudioHandler.WalkStepRatios)
            {
                events.Add(ratio, () => AudioHandler.PlaySound(EntitySoundEvent.Step));
            }
        }

        _animancerState.EffectiveSpeed = _walkSpdMultiplier * _defaultWalkAnimationSpeed;
    }

    public override void Sit()
    {
        _lastAnimationType = HumanoidAnimType.other;
        PlayAnimation((int)HumanoidAnimationEvent.sit);

        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {
            events.Add(0, () => AudioHandler.PlaySound(EntitySoundEvent.Sitdown));
        }
    }

    public override void Die()
    {
        _lastAnimationType = HumanoidAnimType.other;
        PlayAnimation((int)HumanoidAnimationEvent.death);

        _animancerState.EffectiveSpeed = _defaultDieAnimationSpeed;

        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {
            events.Add(AudioHandler.DeathRatio, () => AudioHandler.PlaySound(EntitySoundEvent.Death));
            events.Add(AudioHandler.FallRatio, () => AudioHandler.PlaySound(EntitySoundEvent.Fall));
        }
    }

    public override void SitWait()
    {
        _lastAnimationType = HumanoidAnimType.other;
        PlayAnimation((int)HumanoidAnimationEvent.sit_wait);
    }

    public override void Stand()
    {
        _lastAnimationType = HumanoidAnimType.other;
        PlayAnimation((int)HumanoidAnimationEvent.stand);

        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {
            events.Add(0, () => AudioHandler.PlaySound(EntitySoundEvent.Standup));
        }
    }

    public override bool PlaySkillCastAnimation()
    {
        if (base.PlaySkillCastAnimation())
        {
            if ((int)_skillCastAnimation >= 100)
            {
                Debug.LogWarning("Weapon cast animations not yet handled.");
                return false;
            }

            HumanoidAnimType castAnim = (HumanoidAnimType)_skillCastAnimation;

            // SetBool(castAnim, true);

            return true;
        }

        return false;
    }

    public override bool PlaySkillThrowAnimation()
    {
        if (base.PlaySkillThrowAnimation())
        {
            HumanoidAnimType throwAnim = (HumanoidAnimType)_skillThrowAnimation;

            // SetBool(throwAnim, true);

            return true;
        }

        return false;
    }

}
