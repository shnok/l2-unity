#pragma warning disable 414

using Animancer;
using UnityEngine;

// Used by NPCS and USERS
public class NewHumanoidAnimationController : NewBaseAnimationController
{
    public WeaponAnimType WeaponAnim { get { return _weaponAnim; } }
    public HumanoidWeaponAnimType LastAnimationType { get { return _lastAnimationType; } }
    private HumanoidAudioHandler AudioHandler { get => (HumanoidAudioHandler)_entityReferenceHolder.AudioHandler; }

    [Header("Base Speeds")]
    [SerializeField] private float _defaultIdleAnimationSpeed = 0.3f;
    [SerializeField] private float _defaultAtkWaitAnimationSpeed = 0.5f;
    [SerializeField] private float _defaultRunAnimationSpeed = 0.35f;
    [SerializeField] private float _defaultWalkAnimationSpeed = 0.4f;
    [SerializeField] private float _defaultJumpAnimationSpeed = 1.25f;
    [SerializeField] private float _defaultDieAnimationSpeed = 0.5f;

    [Header("Humanoids")]
    [SerializeField] protected L2HumanoidAnimationContainerDefault _defaultAnimContainer; //TODO: Cache in Singleton?
    [SerializeField] protected L2HumanoidAnimationContainerAtk _atkAnimContainer; //TODO: Cache in Singleton?
    [SerializeField] protected L2HumanoidAnimationContainerSpAtk _spAtkAnimContainer; //TODO: Cache in Singleton?
    [SerializeField] protected L2HumanoidAnimationContainerSocial _socialAnimContainer; //TODO: Cache in Singleton?
    [SerializeField] protected HumanoidWeaponAnimType _lastAnimationType;
    [SerializeField] protected WeaponAnimType _weaponAnim;
    [SerializeField] private int _atkAnimIndex;

    public override void Initialize()
    {
        base.Initialize();
        _lastAnimationType = HumanoidWeaponAnimType.wait;

        if (_defaultAnimContainer == null)
        {
            Debug.LogWarning($"[{transform.name}] L2Animations was not assigned, please pre-assign it.");
        }

        Wait();
    }

    protected override AnimationClip GetAnimationClip(AnimationCategory animationCategory, int index)
    {
        switch (animationCategory)
        {
            case AnimationCategory.Default:
                if (_defaultAnimContainer != null && index < _defaultAnimContainer.Animations.Length)
                    return _defaultAnimContainer.Animations[index].AnimationClip;
                break;
            case AnimationCategory.Atk:
                if (_atkAnimContainer != null && index < _atkAnimContainer.Animations.Length)
                    return _atkAnimContainer.Animations[index].AnimationClip;
                break;
            case AnimationCategory.SpAtk:
                if (_spAtkAnimContainer != null && index < _spAtkAnimContainer.Animations.Length)
                    return _spAtkAnimContainer.Animations[index].AnimationClip;
                break;
            case AnimationCategory.Social:
                if (_socialAnimContainer != null && index < _socialAnimContainer.Animations.Length)
                    return _socialAnimContainer.Animations[index].AnimationClip;
                break;
        }

        return null;
    }

    public override void WeaponAnimChanged(WeaponAnimType newWeaponAnim)
    {
        _weaponAnim = newWeaponAnim;

        if (!((int)_lastAnimationType < (int)HumanoidWeaponAnimType.cast))
        {
            Debug.LogWarning($"The last animation was not a weapon animation: {_lastAnimationType}");
            // The last animation was not a weapon animation
            return;
        }

        // Adding weaponanim index to humanoidanimtype will give the correct animationEvent
        // HumanoidAnimationDefaultEvent newAnim = (HumanoidAnimationDefaultEvent)(int)_lastAnimationType + (int)_weaponAnim;

        Debug.Log($"Last animation type: {_lastAnimationType} Weapon anim: {_weaponAnim}");

        switch (_lastAnimationType)
        {
            case HumanoidWeaponAnimType.wait:
                Wait();
                break;
            case HumanoidWeaponAnimType.walk:
                Walk();
                break;
            case HumanoidWeaponAnimType.run:
                Run();
                break;
            case HumanoidWeaponAnimType.atkwait:
                AtkWait();
                break;
        }

        // PlayAnimation((int)newAnim);
    }

    public override void Attack()
    {
        _lastAnimationType = HumanoidWeaponAnimType.other;
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
        HumanoidAnimationAtkEvent toPlay;
        switch (_weaponAnim)
        {
            case WeaponAnimType._1HS:
                toPlay = HumanoidAnimationAtkEvent.atk01_1HS;
                break;
            case WeaponAnimType._2HS:
                toPlay = HumanoidAnimationAtkEvent.atk01_2HS;
                break;
            case WeaponAnimType.bow:
                toPlay = HumanoidAnimationAtkEvent.atk01_bow;
                break;
            case WeaponAnimType.pole:
                toPlay = HumanoidAnimationAtkEvent.atk01_pole;
                break;
            case WeaponAnimType.dual:
                toPlay = HumanoidAnimationAtkEvent.atk01_dual;
                break;
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
            default:
                toPlay = HumanoidAnimationAtkEvent.atk01_hand;
                break;
        }

        PlayAnimation(AnimationCategory.Atk, (int)toPlay + _atkAnimIndex);
        UpdateAttackAnimationSpeed(_lastPlayedClipDuration, _atkSpd);

        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {

            if (_weaponAnim == WeaponAnimType.bow)
            {
                events.Add(_nockArrowRatio, () =>
                {
                    _entityReferenceHolder.Combat.NockArrow();
                    AudioHandler.PlayBowBendSound();
                });

                events.Add(_shootArrowRatio, () =>
                {
                    _entityReferenceHolder.Combat.ShootArrow();
                    AudioHandler.PlayArrowShootSound();
                });
            }
            else
            {
                events.Add(AudioHandler.AtkRatio, () => AudioHandler.PlayAtkSound());
            }
            events.OnEnd = NextAttack;
        }

        _animancerState.EffectiveSpeed = _atkSpdMultiplier;
    }

    public override void AtkWait()
    {
        _lastAnimationType = HumanoidWeaponAnimType.atkwait;
        switch (_weaponAnim)
        {
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.atkwait_hand);
                break;
            case WeaponAnimType._1HS:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.atkwait_1HS);
                break;
            case WeaponAnimType._2HS:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.atkwait_2HS);
                break;
            case WeaponAnimType.bow:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.atkwait_bow);
                break;
            case WeaponAnimType.pole:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.atkwait_pole);
                break;
            case WeaponAnimType.dual:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.atkwait_dual);
                break;
        }

        _animancerState.EffectiveSpeed = _defaultAtkWaitAnimationSpeed;
    }

    public override void Run()
    {
        _lastAnimationType = HumanoidWeaponAnimType.run;
        switch (_weaponAnim)
        {
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.run_hand);
                break;
            case WeaponAnimType._1HS:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.run_1HS);
                break;
            case WeaponAnimType._2HS:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.run_2HS);
                break;
            case WeaponAnimType.bow:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.run_bow);
                break;
            case WeaponAnimType.pole:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.run_pole);
                break;
            case WeaponAnimType.dual:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.run_dual);
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
        _lastAnimationType = HumanoidWeaponAnimType.wait;
        switch (_weaponAnim)
        {
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.wait_hand);
                break;
            case WeaponAnimType._1HS:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.wait_1HS);
                break;
            case WeaponAnimType._2HS:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.wait_2HS);
                break;
            case WeaponAnimType.bow:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.wait_bow);
                break;
            case WeaponAnimType.pole:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.wait_pole);
                break;
            case WeaponAnimType.dual:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.wait_dual);
                break;
        }

        _animancerState.EffectiveSpeed = _defaultIdleAnimationSpeed;
    }

    public override void Walk()
    {
        _lastAnimationType = HumanoidWeaponAnimType.walk;
        switch (_weaponAnim)
        {
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.walk_hand);
                break;
            case WeaponAnimType._1HS:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.walk_1HS);
                break;
            case WeaponAnimType._2HS:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.walk_2HS);
                break;
            case WeaponAnimType.bow:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.walk_bow);
                break;
            case WeaponAnimType.pole:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.walk_pole);
                break;
            case WeaponAnimType.dual:
                PlayAnimation((int)HumanoidAnimationDefaultEvent.walk_dual);
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
        _lastAnimationType = HumanoidWeaponAnimType.other;
        PlayAnimation((int)HumanoidAnimationDefaultEvent.sit);

        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {
            events.Add(0, () => AudioHandler.PlaySound(EntitySoundEvent.Sitdown));
        }
    }

    public override void Die()
    {
        _lastAnimationType = HumanoidWeaponAnimType.other;
        PlayAnimation((int)HumanoidAnimationDefaultEvent.death);

        _animancerState.EffectiveSpeed = _defaultDieAnimationSpeed;

        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {
            events.Add(AudioHandler.DeathRatio, () => AudioHandler.PlaySound(EntitySoundEvent.Death));
            events.Add(AudioHandler.FallRatio, () => AudioHandler.PlaySound(EntitySoundEvent.Fall));
            events.OnEnd = DieWait;
        }
    }

    public override void DieWait()
    {
        _lastAnimationType = HumanoidWeaponAnimType.other;
        PlayAnimation((int)HumanoidAnimationDefaultEvent.deathwait);
    }

    public override void Resurrect()
    {
    }

    public override void SitWait()
    {
        _lastAnimationType = HumanoidWeaponAnimType.other;
        PlayAnimation((int)HumanoidAnimationDefaultEvent.sit_wait);
    }

    public override void Stand()
    {
        _lastAnimationType = HumanoidWeaponAnimType.other;
        PlayAnimation((int)HumanoidAnimationDefaultEvent.stand);

        if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
        {
            events.Add(0, () => AudioHandler.PlaySound(EntitySoundEvent.Standup));
        }
    }

    public override bool PlaySkillCastAnimation()
    {
        if (base.PlaySkillCastAnimation())
        {
            _lastAnimationType = HumanoidWeaponAnimType.cast;

            SkillCastAnimation skillCastAnimation = _lastSkill.Skillgrps[0].CastAnimation;
            int castAnim = (int)skillCastAnimation;

            if (skillCastAnimation >= SkillCastAnimation.SpAtk01)
            {
                int spAtkIndex = (int)skillCastAnimation - 100;
                HumanoidAnimationSpAtkEvent spAtkEvent = HumanoidAnimationSpAtkEvent.spatk01_1HS;
                switch (_weaponAnim)
                {
                    case WeaponAnimType.shield:
                    case WeaponAnimType.hand:
                        spAtkEvent = HumanoidAnimationSpAtkEvent.spatk01_hand;
                        break;
                    case WeaponAnimType._1HS:
                        spAtkEvent = HumanoidAnimationSpAtkEvent.spatk01_1HS;
                        break;
                    case WeaponAnimType._2HS:
                        spAtkEvent = HumanoidAnimationSpAtkEvent.spatk01_2HS;
                        break;
                    case WeaponAnimType.bow:
                        spAtkEvent = HumanoidAnimationSpAtkEvent.spatk01_bow;
                        break;
                    case WeaponAnimType.pole:
                        spAtkEvent = HumanoidAnimationSpAtkEvent.spatk01_pole;
                        break;
                    case WeaponAnimType.dual:
                        spAtkEvent = HumanoidAnimationSpAtkEvent.spatk01_dual;
                        break;
                }

                castAnim = spAtkIndex + (int)spAtkEvent;
                PlayAnimation(AnimationCategory.SpAtk, castAnim);

                AudioHandler.PlayPreAtkSound(spAtkIndex);

                if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
                {
                    events.Add(0.60f, () => AudioHandler.PlaySpAtkSound());
                    events.OnEnd = Wait;
                }
                UpdateCastAnimationSpeed(_lastPlayedClipDuration, _entityReferenceHolder.Combat.LastSkillHitTime, true);
            }
            else
            {
                PlayAnimation(castAnim);
                if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
                {
                    events.OnEnd = PlaySkillCastEndAnimation;
                }
                UpdateCastAnimationSpeed(_lastPlayedClipDuration, _entityReferenceHolder.Combat.LastSkillHitTime, false);
            }


            AudioHandler.PlaySkillVoice(_lastSkill.SkillSoundgrp.CastingVoices[(int)_entityReferenceHolder.Entity.RaceId]);

            _animancerState.EffectiveSpeed = _castSpdMultiplier;

            return true;
        }

        return false;
    }

    public override void PlaySkillCastEndAnimation()
    {
        PlayAnimation((int)HumanoidAnimationDefaultEvent.castend);
    }

    public override bool PlaySkillThrowAnimation()
    {
        if (base.PlaySkillThrowAnimation())
        {
            _lastAnimationType = HumanoidWeaponAnimType.cast_throw;

            SkillThrowAnimation skillThrowAnim = _lastSkill.Skillgrps[0].ThrowAnimation;

            HumanoidAnimationDefaultEvent throwAnim = (HumanoidAnimationDefaultEvent)skillThrowAnim;

            PlayAnimation((int)throwAnim);

            _animancerState.EffectiveSpeed = _castSpdMultiplier;

            if (_animancerState.Events(null, out AnimancerEvent.Sequence events))
            {
                events.Add(0.35f, () => AudioHandler.PlaySkillVoice(_lastSkill.SkillSoundgrp.CastingEndVoices[(int)_entityReferenceHolder.Entity.RaceId]));
                events.OnEnd = Wait;
            }

            return true;
        }

        return false;
    }
}
