using System;
using System.Collections.Generic;
using UnityEngine;

// Used by LOCAL PLAYER
public class NewPlayerAnimationController : NewHumanoidAnimationController
{
    private static NewPlayerAnimationController _instance;
    public static NewPlayerAnimationController Instance { get { return _instance; } }

    public override void Initialize()
    {
        base.Initialize();

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    // public void SetBool(HumanoidAnimType animType, bool value, bool share)
    // {
    //     int paramId = GetParameterId(animType, _weaponAnim);

    //     if (Animator.GetBool(paramId) != value)
    //     {
    //         // Debug.LogWarning($"Set bool {name}={value}");
    //         SetBool(paramId, value);
    //     }
    // }
}

