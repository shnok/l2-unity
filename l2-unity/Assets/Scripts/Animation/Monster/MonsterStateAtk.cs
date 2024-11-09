using System;
using UnityEngine;

public class MonsterStateAtk : MonsterStateAction
{
    private float clipLength;
    private float _lastNormalizedTime = 0;
    private float _lastArrowNormalizedTime = 0;
    private bool _nockedArrow = false;
    private bool _shotArrow = false;
    public const float SHOOT_ARROW_RATIO = 0.6f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        _lastArrowNormalizedTime = 0;
        _lastArrowNormalizedTime = 0;
        _nockedArrow = false;
        _shotArrow = false;

        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(0);
        if (clipInfos == null || clipInfos.Length == 0)
        {
            clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        }

        clipLength = clipInfos[0].clip.length;

        AnimController.UpdateAnimatorAtkSpdMultiplier(clipLength);

        SetBool(MonsterAnimationEvent.wait, false);
        SetBool(MonsterAnimationEvent.atkwait, false);

        PlaySoundAtRatio(EntitySoundEvent.Atk, AudioHandler.AtkRatio);
        PlaySoundAtRatio(EntitySoundEvent.Swish, AudioHandler.SwishRatio);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > 0.25f)
        {
            SetBool(MonsterAnimationEvent.atk01, false);
        }

        if (IsDead())
        {
            SetBool(MonsterAnimationEvent.atk01, false);
            SetBool(MonsterAnimationEvent.death, true);
            return;
        }

        if (_referenceHolder.Gear.WeaponType == WeaponType.bow)
        {
            ManageArrow(stateInfo);
        }

        if (IsMoving())
        {
            if (Entity.Running)
            {
                SetBool(MonsterAnimationEvent.atk01, false);
                SetBool(MonsterAnimationEvent.run, true);
            }
            else
            {
                SetBool(MonsterAnimationEvent.atk01, false);
                SetBool(MonsterAnimationEvent.walk, true);
            }

            return;
        }

        if (DidAttackTimeout())
        {
            SetBool(MonsterAnimationEvent.atk01, false);
            SetBool(MonsterAnimationEvent.atkwait, true);
        }

        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            PlaySoundAtRatio(EntitySoundEvent.Atk, AudioHandler.AtkRatio);
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
