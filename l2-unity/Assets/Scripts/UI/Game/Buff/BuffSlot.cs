using UnityEngine.UIElements;

public class BuffSlot : L2Slot
{
    public Buff Buff { get; private set; }

    public BuffSlot(VisualElement slotElement, int position, Buff buff) : base(slotElement, position, SlotType.Effect)
    {
        Buff = buff;
    }
    
    public void AssignEffect()
    {
        AddTooltip();
    }
    
    private void AddTooltip()
    {
        _tooltipManipulator?.SetValue(Buff);
    }
}