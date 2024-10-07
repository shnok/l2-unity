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

        Debug.Log($"New Weapon animation index: {newAnimationIndex}");

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
}
