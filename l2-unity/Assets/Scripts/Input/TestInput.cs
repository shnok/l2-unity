using UnityEngine;

public class TestInput : MonoBehaviour
{
    [SerializeField] private int _skillId = 1177;

    void Update()
    {
        if (InputManager.Instance.Test)
        {
            // GameClient.Instance?.ClientPacketHandler.RequestMagicSkillUse(_skillId, _ctrlPressed, _shiftPressed);
            Skill skill = SkillTable.Instance.GetSkill(_skillId);
            SkillInfo skillInfo = new SkillInfo(skill.EffectId, 1, false, false);
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_SKILL, skillInfo);
        }
    }
}
