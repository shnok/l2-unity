using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerStateAtk : PlayerStateAction
{
    private float _lastArrowNormalizedTime = 0;
    private bool _nockedArrow = false;
    private bool _shotArrow = false;
    public const float SHOOT_ARROW_RATIO = 0.6f; //TODO: Change based on race

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        _lastArrowNormalizedTime = 0;
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

        if (_referenceHolder.Gear.WeaponType != WeaponType.bow)
        {
            PlayAtkSoundAtRatio(AudioHandler.AtkRatio);
        }
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
            ManageArrow(stateInfo);
        }

        if (ShouldAttack())
        {
            if (_referenceHolder.Gear.WeaponType == WeaponType.bow)
            {
                // Lock on the target while attacking
                PlayerController.Instance.StartLookAt(TargetManager.Instance.AttackTarget?.Data.ObjectTransform);
            }
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

    private void ManageArrow(AnimatorStateInfo stateInfo)
    {
        float normalizedRatio = stateInfo.normalizedTime - _lastArrowNormalizedTime;

        if (normalizedRatio >= 1f)
        {
            Debug.LogWarning("Reset atk animation state");
            _nockedArrow = false;
            _shotArrow = false;
            _lastArrowNormalizedTime = stateInfo.normalizedTime;
        }
        else if (normalizedRatio >= SHOOT_ARROW_RATIO)
        {
            if (!_shotArrow)
            {
                _shotArrow = true;
                _referenceHolder.Combat.ShootArrow();
                AudioHandler.PlayArrowShootSound();
            }
        }
        else if (normalizedRatio >= 0.2f)
        {
            if (!_nockedArrow)
            {
                _nockedArrow = true;
                _referenceHolder.Combat.NockArrow();
                AudioHandler.PlayBowBendSound();
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_nockedArrow)
        {
            _referenceHolder.Gear.HideArrow();
        }
    }
}