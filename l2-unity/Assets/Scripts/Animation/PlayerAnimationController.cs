using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : BaseAnimationController {
    private static PlayerAnimationController _instance;
    public static PlayerAnimationController Instance { get { return _instance; } }

    protected override void Initialize() {
        base.Initialize();

        if (_instance == null) {
            _instance = this;
        }
    }

    public void SetBool(string name, bool value) {
        if (_animator.GetBool(name) != value) {
            _animator.SetBool(name, value);
            if (!World.Instance.OfflineMode) {
                EmitAnimatorInfo(name, value ? 1 : 0);
            }
        }
    }

    private int SerializeAnimatorInfo(string name) {
        List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>(_animator.parameters);

        int index = parameters.FindIndex(a => a.name == name);

        return index;
    }

    private void EmitAnimatorInfo(string name, float value) {
        int index = SerializeAnimatorInfo(name);
        if(index != -1) {
            _animator.GetComponentInParent<NetworkTransformShare>().ShareAnimation((byte)index, value);
        }
    }
}

