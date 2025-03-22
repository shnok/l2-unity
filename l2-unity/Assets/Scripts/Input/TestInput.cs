using UnityEngine;

public class TestInput : MonoBehaviour
{
    [SerializeField] private int _skillId = 1177;
    [SerializeField] private bool _ctrlPressed = true;
    [SerializeField] private bool _shiftPressed = false;

    void Update()
    {
        if (InputManager.Instance.Test)
        {
            // GameClient.Instance?.ClientPacketHandler.RequestMagicSkillUse(_skillId, _ctrlPressed, _shiftPressed);
            Skill skill = SkillTable.Instance.GetSkill(_skillId);
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_SKILL, skill);
        }
    }
}
