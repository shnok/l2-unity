using UnityEngine;

public class MonsterStateWait : MonsterStateAction
{
    public int playBreatheSoundChancePercent = 100;
    bool started = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        if (RandomUtils.ShouldEventHappen(playBreatheSoundChancePercent))
        {
            AudioHandler.PlaySound(EntitySoundEvent.Wait);
        }
        SetBool(MonsterAnimationEvent.spwait, false);
        started = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (IsDead())
        {
            SetBool(MonsterAnimationEvent.death, true);
            return;
        }
        if (IsMoving())
        {
            if (Entity.Running)
            {
                SetBool(MonsterAnimationEvent.run, true);
            }
            else
            {
                SetBool(MonsterAnimationEvent.walk, true);
            }
            return;
        }

        if ((stateInfo.normalizedTime % 1) >= 0.90f && started)
        {
            started = false;
            SetBool(MonsterAnimationEvent.wait, true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
