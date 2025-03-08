using UnityEngine;

public class HumanoidStateCast : HumanoidStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);

        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(0);
        if (clipInfos == null || clipInfos.Length == 0)
        {
            clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        }

        AnimController.SetMAtkSpd(clipInfos[0].clip.length);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.castlong, false);
        SetBool(HumanoidAnimType.castmid, false);
        SetBool(HumanoidAnimType.castshort, false);

        if (IsDead())
        {
            SetBool(HumanoidAnimType.castlong, false);
            SetBool(HumanoidAnimType.castmid, false);
            SetBool(HumanoidAnimType.castshort, false);
            SetBool(HumanoidAnimType.death, true);
            return;
        }

        if (IsMoving())
        {
            SetBool(HumanoidAnimType.castlong, false);
            SetBool(HumanoidAnimType.castmid, false);
            SetBool(HumanoidAnimType.castshort, false);

            if (Entity.Running)
            {
                SetBool(HumanoidAnimType.run, true);
            }
            else
            {
                SetBool(HumanoidAnimType.walk, true);
            }

            return;
        }
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}