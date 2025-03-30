using UnityEngine.UIElements;

public class SkillSlot : L2DraggableSlot
{
    private SlotClickSoundManipulator _slotClickSoundManipulator;
    public SkillWindowInfo Skill { get; private set; }

    public SkillSlot(int position, VisualElement slotElement, SlotType slotType)
        : base(position, slotElement, slotType, false, true)
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
}