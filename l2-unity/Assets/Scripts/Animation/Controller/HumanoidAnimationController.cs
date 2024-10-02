using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnimationController : BaseAnimationController
{
    private string _lastAnimationVariableName;

    public override void Initialize()
    {
        base.Initialize();
        _lastAnimationVariableName = "wait_hand";
    }

    public override void WeaponAnimChanged(string newWeaponAnim)
    {
        ClearAnimParams();

        if (!_lastAnimationVariableName.Contains("_"))
        {
            Debug.LogWarning($"The last animation was not a weapon animation: {_lastAnimationVariableName}");
            // The last animation was not a weapon animation
            return;
        }

        string[] parts = _lastAnimationVariableName.Split("_");
        if (parts.Length < 1)
        {
            // Should not happen
            Debug.LogWarning($"Error while parsing previous animation name: {_lastAnimationVariableName}");
            return;
        }

        string newAnimation = parts[0] + "_" + newWeaponAnim;
        Debug.Log($"New Weapon animation name: {newAnimation}");
        SetBool(newAnimation, true);
    }

    public override void SetBool(string name, bool value)
    {
        if (value == true)
        {
            _lastAnimationVariableName = name;
        }

        base.SetBool(name, value);
    }
}
