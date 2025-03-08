using System;
using UnityEngine;

public class HumanoidStateCastEnd : HumanoidStateAction
{
    private bool _skillLaunched = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        _skillLaunched = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.castend, false);
        if (IsDead())
        {
            return;
        }

        if (IsMoving())
        {
            return;
        }

        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        int hitTime = (int)(_referenceHolder.Combat.LastSkillHitTime * 0.75f);
        long endTime = _referenceHolder.Combat.LastSkillUseTime + hitTime;

        if (now > endTime && !_skillLaunched) //Play launch animation at 75%
        {
            Debug.Log(_referenceHolder.Combat.LastSkillUseTime);
            Debug.Log(hitTime);
            Debug.Log(now + " - " + endTime + " - " + (now - endTime));
            _skillLaunched = true;
            _referenceHolder.Combat.LaunchSkill();
        }
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}