using UnityEngine;

public class HumanoidStateStand : HumanoidStateBase
{
    private float _lastNormalizedTime = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        SetBool(HumanoidAnimType.stand, false);
        _lastNormalizedTime = 0;
        AudioHandler.PlaySound(EntitySoundEvent.Standup);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetBool(HumanoidAnimType.stand, false);
        if ((stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            SetBool(HumanoidAnimType.wait, true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
