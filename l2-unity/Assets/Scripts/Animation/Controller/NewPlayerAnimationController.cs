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
}

