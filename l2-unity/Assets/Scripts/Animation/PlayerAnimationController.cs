using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : BaseAnimationController {
    private static PlayerAnimationController _instance;
    public static PlayerAnimationController Instance { get { return _instance; } }

    public override void Initialize() {
        base.Initialize();

        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    void OnDestroy() {
        _instance = null;
    }

    public void SetBool(string name, bool value, bool share) {
        if (_animator.GetBool(name) != value) {
            //Debug.LogWarning($"Set bool {name}={value}");
            _animator.SetBool(name, value);
            if (!World.Instance.OfflineMode && share) {
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

