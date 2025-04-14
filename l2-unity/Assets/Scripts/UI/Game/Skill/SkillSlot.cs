using UnityEngine.UIElements;
using UnityEngine;

public class SkillSlot : L2DraggableSlot
{
    protected SkillInfo _skillInfo;
    protected bool _toggled;
    protected ButtonClickSoundManipulator _buttonClickSoundManipulator;
    protected SlotAnimationManipulator _slotAnimationManipulator;
    public bool Toggled { get => _toggled; set => _toggled = value; }

    public SkillWindowInfo Skill { get; private set; }
    public SkillInfo SkillInfo { get => _skillInfo; set => _skillInfo = value; }
    public SlotAnimationManipulator SlotAnimationManipulator { get => _slotAnimationManipulator; }

    public SkillSlot(int position, VisualElement slotElement, SlotType slotType)
        : base(position, slotElement, slotType, true, false)
    {

    }

    public void AssignSkill(SkillWindowInfo skill)
    {
        _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        _slotElement.AddManipulator(_buttonClickSoundManipulator);

        _slotAnimationManipulator = new SlotAnimationManipulator(_slotElement, this);

        StyleBackground background = new StyleBackground(IconTable.Instance.LoadTextureByName(skill.Icon));
        _slotBg.style.backgroundImage = background;

        _slotElement.RemoveFromClassList("empty");
        _slotDragManipulator.enabled = true;
        _id = skill.SkillId;

        _skillInfo = PlayerSkill.Instance.GetSkillInfo(_id);

        Skill = skill;
        AddTooltip();
    }

    public override void ClearManipulators()
    {
        base.ClearManipulators();

        if (_buttonClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_buttonClickSoundManipulator);
            _buttonClickSoundManipulator = null;
        }

        if (_slotAnimationManipulator != null)
        {
            _slotElement.RemoveManipulator(_slotAnimationManipulator);
            _slotAnimationManipulator = null;
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