using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateBase : StateMachineBehaviour
{
    public PlayerController pc;
   // public CombatController cc;
	public Animator _animator;
	public bool owned;

    public void LoadComponents(Animator animator) {
        if(!pc)
            pc = animator.GetComponentInParent<PlayerController>();
		//if(!cc)
		//	cc = animator.GetComponentInParent<CombatController>();
		if(!_animator)
			_animator = animator;

	//	owned = animator.GetComponentInParent<Entity>().Identity.Owned;
	}

	public void SetBool(string name, bool value) {
		if(!owned)
			return;

		if(_animator.GetBool(name) != value) {
			_animator.SetBool(name, value);

			EmitAnimatorInfo(name, value ? 1 : 0);
		}
	}

	public void SetFloat(string name, float value) {
		if(!owned)
			return;

		if(Mathf.Abs(_animator.GetFloat(name) - value) > 0.2f) {
			_animator.SetFloat(name, value);

			EmitAnimatorInfo(name, value);
		}
	}

	public void SetInteger(string name, int value) {
		if(!owned)
			return;

		if(_animator.GetInteger(name) != value) {
			_animator.SetInteger(name, value);

			EmitAnimatorInfo(name, value);
		}
	}

	public void SetTrigger(string name) {
		if(!owned)
			return;

		_animator.SetTrigger(name);

		EmitAnimatorInfo(name, 0);
	}

	private int SerializeAnimatorInfo(string name) {
		List<AnimatorControllerParameter> parameters  = new List<AnimatorControllerParameter>(_animator.parameters);

		int index = parameters.FindIndex(a => a.name == name);

		return index;
	}

	private void EmitAnimatorInfo(string name, float value) {
		if(owned) {
			int index = SerializeAnimatorInfo(name);
			if(index != -1) {
				//_animator.GetComponentInParent<NetworkTransformShare>().ShareAnimation((byte)index, value);
            }
        }
	}
}
