using UnityEngine;

public class MonsterStateSpWait : MonsterStateAction
{
    public int playSpWaitChancePercent = 10;
    private bool hasStarted = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        hasStarted = true;
        SetBool(MonsterAnimationEvent.wait, false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(MonsterAnimationEvent.wait, false);

        if (Entity.IsDead)
        {
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
        }
        else if (hasStarted && (stateInfo.normalizedTime % 1) < 0.5f)
        {
            SetBool(MonsterAnimationEvent.wait, false);

            if (RandomUtils.ShouldEventHappen(AudioHandler.WaitSoundChance))
            {
                AudioHandler.PlaySound(EntitySoundEvent.Breathe);
            }
            hasStarted = false;
        }
        else if (!hasStarted && (stateInfo.normalizedTime % 1) >= 0.90f)
        {
            if (RandomUtils.ShouldEventHappen(playSpWaitChancePercent))
            {
                SetBool(MonsterAnimationEvent.spwait, true);
            }
            hasStarted = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
