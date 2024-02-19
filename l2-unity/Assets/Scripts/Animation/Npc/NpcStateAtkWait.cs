using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStateAtkWait : NpcStateBase {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        audioHandler.PlaySoundAtRatio(CharacterSoundEvent.AtkWait, audioHandler.AtkWaitRatio);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
