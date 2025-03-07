using System.Collections.Generic;
using UnityEngine;

// Used by NPCS and USERS
public class HumanoidAnimationController : BaseAnimationController
{
    [SerializeField] protected HumanoidAnimType _lastAnimationType;
    [SerializeField] protected WeaponAnimType _weaponAnim;
    private int _lastAnimIndex = -1;
    private bool _lastValue = false;

    public WeaponAnimType WeaponAnim { get { return _weaponAnim; } }
    public HumanoidAnimType LastAnim { get { return _lastAnimationType; } }

    public override void Initialize()
    {
        base.Initialize();
        _lastAnimationType = HumanoidAnimType.wait;
    }

    public override void WeaponAnimChanged(WeaponAnimType newWeaponAnim)
    {
        ClearAnimParams();

        _weaponAnim = newWeaponAnim;

        if (!((int)_lastAnimationType < (int)HumanoidAnimType.wait_hit))
        {
            Debug.LogWarning($"The last animation was not a weapon animation: {_lastAnimationType}");
            // The last animation was not a weapon animation
            return;
        }

        int newAnimationIndex = GetParameterId(_lastAnimationType, _weaponAnim);

        // Debug.Log($"New Weapon animation index: {newAnimationIndex} Last animation type: {_lastAnimationType} Weapon anim: {_weaponAnim}");

        SetBool(newAnimationIndex, true);
    }

    public override void UpdateAnimatorAtkSpdMultiplier(float clipLength)
    {
        float newAtkSpd = clipLength * 1000f / _pAtkSpd;
        Animator.SetFloat(GetParameterId(HumanoidAnimationEvent.patkspd), newAtkSpd);
    }

    public override void SetMAtkSpd(float value)
    {
        //TODO: update for cast animation
        float newMAtkSpd = _spAtk01ClipLength / value;
        Animator.SetFloat(GetParameterId(HumanoidAnimationEvent.matkspd), newMAtkSpd);
    }

    public override void SetRunSpeed(float value)
    {
        Animator.SetFloat(GetParameterId(HumanoidAnimationEvent.run_speed), value);
    }

    public override void SetWalkSpeed(float value)
    {
        Animator.SetFloat(GetParameterId(HumanoidAnimationEvent.walk_speed), value);
    }

    public void SetBool(HumanoidAnimType animType, bool value)
    {
        int paramId = GetParameterId(animType, _weaponAnim);
        if (paramId == _lastAnimIndex && value == _lastValue)
        {
            return;
        }

        _lastAnimIndex = paramId;
        _lastValue = value;
        _lastAnimationType = animType;

        // Debug.LogWarning($"{transform.name} - SetBool: {animType}={value}");

        base.SetBool(GetParameterId(animType, _weaponAnim), value);
    }

    public bool GetBool(HumanoidAnimType animType)
    {
        return base.GetBool(GetParameterId(animType, _weaponAnim));
    }

    protected int GetParameterId(HumanoidAnimType animType, WeaponAnimType weaponAnimType)
    {
        int index = (int)animType;
        if ((int)animType < (int)HumanoidAnimType.wait_hit)
        {
            index = (int)animType + (int)weaponAnimType;
        }

        return AnimatorParameterHashTable.GetHumanoidParameterHash(index);
    }

    protected int GetParameterId(HumanoidAnimationEvent animType)
    {
        return AnimatorParameterHashTable.GetHumanoidParameterHash((int)animType);
    }

    public override bool PlayCastAnimation(SkillAnimation animation)
    {
        if (base.PlayCastAnimation(animation))
        {
            HumanoidAnimType castAnim;
            switch (animation)
            {
                case SkillAnimation.CastMid_NoTarget:
                case SkillAnimation.CastMid_Shot:
                case SkillAnimation.CastMid_Throw:
                    castAnim = HumanoidAnimType.castmid;
                    break;
                case SkillAnimation.CastShort_NoTarget:
                case SkillAnimation.CastShort_Shot:
                case SkillAnimation.CastShort_Throw:
                    castAnim = HumanoidAnimType.castshort;
                    break;
                case SkillAnimation.CastLong_NoTarget:
                    castAnim = HumanoidAnimType.castlong;
                    break;
                case SkillAnimation.NoCast_NoTarget:
                    castAnim = HumanoidAnimType.castend;
                    break;
                case SkillAnimation.WarriorBuff01:
                    castAnim = HumanoidAnimType.buff01;
                    break;
                default:
                    return false;
            }

            SetBool(castAnim, true);
        }

        return false;
    }

    public override bool PlayThrowAnimation(SkillAnimation animation)
    {
        if (base.PlayThrowAnimation(animation))
        {
            HumanoidAnimType throwAnim;
            switch (animation)
            {
                case SkillAnimation.NoCast_NoTarget:
                case SkillAnimation.CastShort_NoTarget:
                case SkillAnimation.CastMid_NoTarget:
                case SkillAnimation.CastLong_NoTarget:
                    throwAnim = HumanoidAnimType.magic_no_target;
                    break;
                case SkillAnimation.CastMid_Shot:
                case SkillAnimation.CastShort_Shot:
                    throwAnim = HumanoidAnimType.magic_shot;
                    break;
                case SkillAnimation.CastMid_Throw:
                case SkillAnimation.CastShort_Throw:
                    throwAnim = HumanoidAnimType.magic_throw;
                    break;
                default:
                    return false;
            }

            SetBool(throwAnim, true);
        }

        return false;
    }
}
