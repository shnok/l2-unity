using System;
using System.Collections.Generic;
using UnityEngine;

// Used by LOCAL PLAYER
public class PlayerAnimationController : HumanoidAnimationController
{
    private static PlayerAnimationController _instance;
    public static PlayerAnimationController Instance { get { return _instance; } }

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

    public void SetBool(HumanoidAnimType animType, bool value, bool share)
    {
        int paramId = GetParameterId(animType, _weaponAnim);

        if (Animator.GetBool(paramId) != value)
        {
            //Debug.LogWarning($"Set bool {name}={value}");
            SetBool(paramId, value);

            if (!World.Instance.OfflineMode && share)
            {
                EmitAnimatorInfo(paramId, value ? 1 : 0);
            }
        }
    }

    private void EmitAnimatorInfo(int paramId, float value)
    {
        NetworkTransformShare.Instance.ShareAnimation((byte)paramId, value);
    }
}

