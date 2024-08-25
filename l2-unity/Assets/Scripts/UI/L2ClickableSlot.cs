
using UnityEngine;
using UnityEngine.UIElements;

public class L2ClickableSlot : L2Slot
{
    private bool _leftMouseUp;
    private bool _rightMouseup;

    public L2ClickableSlot(VisualElement slotElement, int position, SlotType type, bool leftMouseUp, bool rightMouseup) : base(slotElement, position, type)
    {
        _leftMouseUp = leftMouseUp;
        _rightMouseup = rightMouseup;
        RegisterClickableCallback();
    }

    protected void RegisterClickableCallback()
    {
        if (_slotElement == null)
        {
            return;
        }

        _slotElement.RegisterCallback<MouseDownEvent>(HandleSlotClickDown, TrickleDown.TrickleDown);
        _slotElement.RegisterCallback<MouseUpEvent>(HandleSlotClickUp, TrickleDown.TrickleDown);
    }

    public void UnregisterClickableCallback()
    {
        if (_slotElement == null)
        {
            return;
        }

        _slotElement.UnregisterCallback<MouseDownEvent>(HandleSlotClickDown, TrickleDown.TrickleDown);
        _slotElement.UnregisterCallback<MouseUpEvent>(HandleSlotClickUp, TrickleDown.TrickleDown);
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

    private void HandleSlotClickDown(MouseDownEvent evt)
    {
        if (evt.button == 0)
        {
            if (!_leftMouseUp)
            {
                HandleLeftClick();
            }
        }
        else if (evt.button == 1)
        {
            if (!_rightMouseup)
            {
                HandleRightClick();
            }
        }
        else
        {
            HandleMiddleClick();
        }
    }

    private void HandleSlotClickUp(MouseUpEvent evt)
    {
        if (!_hoverManipulator.Hovering)
        {
            return;
        }

        if (evt.button == 0)
        {
            if (_leftMouseUp)
            {
                HandleLeftClick();
            }
        }
        else if (evt.button == 1)
        {
            if (_rightMouseup)
            {
                HandleRightClick();
            }
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