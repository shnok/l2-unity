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

        if (IsMoving())
        {
            SetBool(MonsterAnimationEvent.run, true);
        }

        if (!_cancelAction)
        {
            if (!ShouldAtkWait())
            {
                Debug.Log("Wait now");
                SetBool(MonsterAnimationEvent.wait, true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(MonsterAnimationEvent.atkwait, false);
    }
}
