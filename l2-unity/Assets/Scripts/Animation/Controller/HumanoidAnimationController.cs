using System.Collections.Generic;
using UnityEngine;

// Used by NPCS and USERS
public class HumanoidAnimationController : BaseAnimationController
{
    [SerializeField] protected HumanoidAnimType _lastAnimationType;
    [SerializeField] protected WeaponAnimType _weaponAnim;

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

    // public override void SetBool(string name, bool value)
    // {
    //     if (value == true)
    //     {
    //         _lastAnimationVariableName = name;
    //     }

    //     base.SetBool(name, value);
    // }

    public void SetBool(HumanoidAnimType animType, bool value)
    {
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
}
