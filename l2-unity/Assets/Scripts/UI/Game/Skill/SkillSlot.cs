using UnityEngine.UIElements;
using UnityEngine;

public class SkillSlot : L2DraggableSlot
{
    private SlotClickSoundManipulator _slotClickSoundManipulator;
    public SkillWindowInfo Skill { get; private set; }

    public SkillSlot(int position, VisualElement slotElement, SlotType slotType)
        : base(position, slotElement, slotType, true, false)
    {
        _slotClickSoundManipulator = new SlotClickSoundManipulator(_slotElement);
        _slotElement.AddManipulator(_slotClickSoundManipulator);
    }

    public void AssignSkill(SkillWindowInfo skill)
    {
        StyleBackground background = new StyleBackground(IconTable.Instance.LoadTextureByName(skill.Icon));
        _slotBg.style.backgroundImage = background;

        _slotElement.RemoveFromClassList("empty");
        _slotDragManipulator.enabled = true;
        _id = skill.SkillId;

        Skill = skill;
        AddTooltip();
    }

    public override void ClearManipulators()
    {
        base.ClearManipulators();

        if (_slotClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_slotClickSoundManipulator);
            _slotClickSoundManipulator = null;
        }
    }

    private void AddTooltip()
    {
        _tooltipManipulator?.SetValue(Skill);
    }

    protected override void HandleLeftClick()
    {
        if (Skill == null || Skill.IsPassiveSkill())
        {
            return;
        }

        Debug.LogWarning($"Use bar slot {_position}.");
        PlayerSkill.Instance.UseSkill(Skill.SkillId);
    }
}