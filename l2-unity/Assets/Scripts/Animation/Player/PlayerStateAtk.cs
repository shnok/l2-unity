using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerStateAtk : PlayerStateAction
{
    private float _lastNormalizedTime = 0;
    private bool _nockedArrow = false;
    private bool _shotArrow = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        _lastNormalizedTime = 0;
        _nockedArrow = false;
        _shotArrow = false;

        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(0);
        if (clipInfos == null || clipInfos.Length == 0)
        {
            clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        }

        PlayerAnimationController.Instance.UpdateAnimatorAtkSpdMultiplier(clipInfos[0].clip.length);

        SetBool(HumanoidAnimType.atkwait, false, false);
        SetBool(HumanoidAnimType.atk01, false, false);

        PlayAtkSoundAtRatio(AudioHandler.AtkRatio);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.atkwait, false, false);
        SetBool(HumanoidAnimType.atk01, false, false);

        if (ShouldDie())
        {
            return;
        }

        // Safety for when player is still attacking but no new attack packet received
        if (DidAttackTimeout())
        {
            SetBool(HumanoidAnimType.atkwait, true);
            return;
        }

        if (_referenceHolder.Gear.WeaponType == WeaponType.bow)
        {
            float normalizedRatio = stateInfo.normalizedTime - _lastNormalizedTime;

            if (normalizedRatio >= 1f)
            {
                Debug.LogWarning("Reset atk animation state");
                _nockedArrow = false;
                _shotArrow = false;
                _lastNormalizedTime = stateInfo.normalizedTime;
            }
            else if (normalizedRatio >= 0.95f)
            {
                if (!_shotArrow)
                {
                    Debug.LogWarning("Shoot arrow");
                    _shotArrow = true;
                }
            }
            else if (normalizedRatio >= 0.2f)
            {
                if (!_nockedArrow)
                {
                    Debug.LogWarning("Nock arrow");
                    _nockedArrow = true;
                }
            }
        }

        if (ShouldAttack())
        {
            return;
        }

        if (ShouldRun())
        {
            return;
        }

        if (ShouldWalk())
        {
            return;
        }

        if (ShouldSit())
        {
            return;
        }

        if (ShouldIdle())
        {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}