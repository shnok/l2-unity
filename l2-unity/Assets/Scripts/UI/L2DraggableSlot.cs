using System;
using UnityEngine.UIElements;

public class L2DraggableSlot : L2Slot
{
    protected SlotDragManipulator _slotDragManipulator;

    public L2DraggableSlot(int position, VisualElement slotElement, SlotType slotType) : base(slotElement, position, slotType)
    {
        if (slotElement == null)
        {
            return;
        }

        if (_slotDragManipulator == null)
        {
            _slotDragManipulator = new SlotDragManipulator(_slotElement, this);
            _slotElement.AddManipulator(_slotDragManipulator);
        }
    }

    public override void ClearManipulators()
    {
        base.ClearManipulators();

        if (_slotElement == null)
        {
            return;
        }

        if (_slotDragManipulator != null)
        {
            _slotElement.RemoveManipulator(_slotDragManipulator);
            _slotDragManipulator = null;
        }
    }
}