using System.Collections.Generic;
using UnityEngine;

// Used by NPCS and USERS
public class NewHumanoidAnimationController : NewBaseAnimationController
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
        // ClearAnimParams();

        _weaponAnim = newWeaponAnim;

        if (!((int)_lastAnimationType != (int)HumanoidAnimType.other))
        {
            Debug.LogWarning($"The last animation was not a weapon animation: {_lastAnimationType}");
            // The last animation was not a weapon animation
            return;
        }

        // int newAnimationIndex = GetParameterId(_lastAnimationType, _weaponAnim);

        Debug.Log($"New Weapon animation index: {0} Last animation type: {_lastAnimationType} Weapon anim: {_weaponAnim}");

        // SetBool(newAnimationIndex, true);
    }

    private int GetParameterId(HumanoidAnimType animType, WeaponAnimType weaponAnimType)
    {
        int index = (int)animType;
        if ((int)animType != (int)HumanoidAnimType.other)
        {
            index = (int)animType + (int)weaponAnimType;
        }

        return index;
    }


    public override void UpdateAnimatorAtkSpdMultiplier(float clipLength, float patkspd)
    {
        float newAtkSpd = clipLength * 1000f / patkspd;
        _atkSpdMultiplier = newAtkSpd;
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

    // public void SetBool(HumanoidAnimType animType, bool value)
    // {
    //     int paramId = GetParameterId(animType, _weaponAnim);
    //     if (paramId == _lastAnimIndex && value == _lastValue)
    //     {
    //         return;
    //     }

    //     _lastAnimIndex = paramId;
    //     _lastValue = value;
    //     _lastAnimationType = animType;

    //     // Debug.LogWarning($"{transform.name} - SetBool: {animType}={value}");

    //     base.SetBool(GetParameterId(animType, _weaponAnim), value);
    // }

    // public bool GetBool(HumanoidAnimType animType)
    // {
    //     return base.GetBool(GetParameterId(animType, _weaponAnim));
    // }

    // protected int GetParameterId(HumanoidAnimType animType, WeaponAnimType weaponAnimType)
    // {
    //     int index = (int)animType;
    //     if ((int)animType < (int)HumanoidAnimType.wait_hit)
    //     {
    //         index = (int)animType + (int)weaponAnimType;
    //     }

    //     return AnimatorParameterHashTable.GetHumanoidParameterHash(index);
    // }

    // protected int GetParameterId(HumanoidAnimationEvent animType)
    // {
    //     return AnimatorParameterHashTable.GetHumanoidParameterHash((int)animType);
    // }

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

    public override void Attack()
    {
        switch (_weaponAnim)
        {
            case WeaponAnimType.shield:
            case WeaponAnimType.hand:
                PlayAnimation((int)HumanoidAnimationEvent.atk01_hand);
                break;
            case WeaponAnimType._1HS:
                PlayAnimation((int)HumanoidAnimationEvent.atk01_1HS);
                break;
            case WeaponAnimType._2HS:
                PlayAnimation((int)HumanoidAnimationEvent.atk01_2HS);
                break;
            case WeaponAnimType.bow:
                PlayAnimation((int)HumanoidAnimationEvent.atk01_bow);
                break;
            case WeaponAnimType.pole:
                PlayAnimation((int)HumanoidAnimationEvent.atk01_pole);
                break;
            case WeaponAnimType.dual:
                PlayAnimation((int)HumanoidAnimationEvent.atk01_dual);
                break;
        }
    }

    public override void Run()
    {
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
    }

    public override void Wait()
    {
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
    }

    public override void Walk()
    {
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
    }

    public override void Sit()
    {
        PlayAnimation((int)HumanoidAnimationEvent.sit);
    }

    public override void Die()
    {
        PlayAnimation((int)HumanoidAnimationEvent.death);
    }

    public override void SitWait()
    {
        PlayAnimation((int)HumanoidAnimationEvent.sit_wait);
    }

    public override void Stand()
    {
        PlayAnimation((int)HumanoidAnimationEvent.stand);
    }
}
