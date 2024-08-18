using UnityEngine;

public class MonsterStateSpWait : MonsterStateBase
{
    public int playSpWaitChancePercent = 10;
    private bool hasStarted = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        hasStarted = true;
        SetBool("wait", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(hasStarted && (stateInfo.normalizedTime % 1) < 0.5f) {
            SetBool("wait", false);

            if(RandomUtils.ShouldEventHappen(audioHandler.WaitSoundChance)) {
                audioHandler.PlaySound(MonsterSoundEvent.Breathe);
            }
            hasStarted = false;
        }

        if(!hasStarted && (stateInfo.normalizedTime % 1) >= 0.90f) {
            if(RandomUtils.ShouldEventHappen(playSpWaitChancePercent)) {
                SetBool("spwait", true);
            }
            hasStarted = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
