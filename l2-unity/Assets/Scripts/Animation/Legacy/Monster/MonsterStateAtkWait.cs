using System.Collections;
using UnityEngine;

public class MonsterStateAtkWait : MonsterStateAction
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        _cancelAction = false;

        SetBool(MonsterAnimationEvent.atkwait, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(MonsterAnimationEvent.atkwait, false);
        if (IsDead())
        {
            SetBool(MonsterAnimationEvent.death, true);
            return;
        }
        if (IsMoving())
        {
            SetBool(MonsterAnimationEvent.run, true);
        }

        if (!_cancelAction)
        {
            if (!ShouldAtkWait())
            {
                SetBool(MonsterAnimationEvent.wait, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(MonsterAnimationEvent.wait, false);
        SetBool(MonsterAnimationEvent.atkwait, false);
    }
}
