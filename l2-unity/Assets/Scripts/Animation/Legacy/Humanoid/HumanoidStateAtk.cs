using UnityEngine;

public class HumanoidStateAtk : HumanoidStateAction
{
    private float _lastNormalizedTime;
    private float _lastArrowNormalizedTime;
    private bool _nockedArrow = false;
    private bool _shotArrow = false;
    public const float SHOOT_ARROW_RATIO = 0.6f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(0);
        if (clipInfos == null || clipInfos.Length == 0)
        {
            clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        }

        _lastNormalizedTime = 0;
        _lastArrowNormalizedTime = 0;
        _nockedArrow = false;
        _shotArrow = false;

        AnimController.UpdateAnimatorAtkSpdMultiplier(clipInfos[0].clip.length);

        SetBool(HumanoidAnimType.wait, false);
        SetBool(HumanoidAnimType.atkwait, false);
        // SetBool(HumanoidAnimType.atk01, false);

        PlayAtkSoundAtRatio(AudioHandler.AtkRatio);
        // PlaySoundAtRatio(ItemSoundEvent.sword_small, AudioHandler.SwishRatio);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > 0.25f)
        {
            SetBool(HumanoidAnimType.atk01, false);
        }

        if (IsDead())
        {
            SetBool(HumanoidAnimType.atk01, false);
            SetBool(HumanoidAnimType.death, true);
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
                SetBool(HumanoidAnimType.atk01, false);
                SetBool(HumanoidAnimType.run, true);
            }
            else
            {
                SetBool(HumanoidAnimType.atk01, false);
                SetBool(HumanoidAnimType.walk, true);
            }

            return;
        }

        if (DidAttackTimeout())
        {
            SetBool(HumanoidAnimType.atk01, false);
            SetBool(HumanoidAnimType.atkwait, true);
        }

        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            PlayAtkSoundAtRatio(AudioHandler.AtkRatio);
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