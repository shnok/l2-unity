
using UnityEngine;
using UnityEngine.UIElements;

public class L2ClickableSlot : L2Slot
{
    public L2ClickableSlot(VisualElement slotElement, int position, SlotType type) : base(slotElement, position, type)
    {
        RegisterCallbacks();
    }

    protected void RegisterCallbacks()
    {
        if (_slotElement == null)
        {
            return;
        }

        _slotElement.RegisterCallback<MouseDownEvent>(HandleSlotClick, TrickleDown.TrickleDown);
    }

    public void UnregisterCallbacks()
    {
        if (_slotElement == null)
        {
            return;
        }

        _slotElement.UnregisterCallback<MouseDownEvent>(HandleSlotClick, TrickleDown.TrickleDown);
    }

    public void SetSelected()
    {
        Debug.Log($"Slot {_position} selected.");
        _slotElement.AddToClassList("selected");
    }

    public void UnSelect()
    {
        Debug.Log($"Slot {_position} unselected.");
        _slotElement.RemoveFromClassList("selected");
    }

    protected void HandleSlotClick(MouseDownEvent evt)
    {
        if (evt.button == 0)
        {
            HandleLeftClick();
        }
        else if (evt.button == 1)
        {
            HandleRightClick();
        }
        else
        {
            HandleMiddleClick();
        }
    }

    protected virtual void HandleLeftClick() { }
    protected virtual void HandleRightClick() { }
    protected virtual void HandleMiddleClick() { }
}