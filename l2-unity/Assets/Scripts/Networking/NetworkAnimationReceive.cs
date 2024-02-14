using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAnimationReceive : BaseAnimationController {
    protected override void Initialize () {
        if(World.Instance.OfflineMode) {
            this.enabled = false;
            return;
        }

        base.Initialize();
    }

    public void SetAnimationProperty(int animId, float value) {
        SetAnimationProperty(animId, value, false);
    }

    public void SetAnimationProperty(int animId, float value, bool forceReset) {
        if (animId >= 0 && animId < _animator.parameters.Length) {
            if (_resetStateOnReceive || forceReset) {
                ClearAnimParams();
            }

            AnimatorControllerParameter anim = _animator.parameters[animId];

            switch (anim.type) {
                case AnimatorControllerParameterType.Float:
                    _animator.SetFloat(anim.name, value);
                    break;
                case AnimatorControllerParameterType.Int:
                    _animator.SetInteger(anim.name, (int)value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    _animator.SetBool(anim.name, (int)value == 1);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    _animator.SetTrigger(anim.name);
                    break;
            }
        }
    }
}
