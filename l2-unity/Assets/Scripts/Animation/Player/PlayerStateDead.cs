using UnityEngine;

public class PlayerStateDead : PlayerStateBase {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LoadComponents(animator);
        if (!_enabled) {
            return;
        }

        SetBool("death", false, false);
        //PlayerController.Instance.SetCanMove(false);
        PlaySoundAtRatio(CharacterSoundEvent.Death, _audioHandler.DeathRatio);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetBool("death", false, false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
