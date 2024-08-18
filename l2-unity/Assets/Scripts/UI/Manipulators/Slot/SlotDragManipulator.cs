using UnityEngine;
using UnityEngine.UIElements;

public class SlotDragManipulator : PointerManipulator
{
    private VisualElement _root;
    private Vector2 _startMousePosition;
    private Vector2 _startPosition;
    private L2Slot _slot;
    public bool dragged = false;
    public bool enabled = false;

    public SlotDragManipulator(VisualElement target, L2Slot slot)
    {
        _slot = slot;
        _root = target;
        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
    }

    public void PointerDownHandler(PointerDownEvent evt)
    {
        if (!enabled)
        {
            return;
        }

        if (evt.button == 0)
        {
            UpdateFlotatingSlotAppearance();
            dragged = false;
            _startMousePosition = evt.position;
            _startPosition = _root.worldBound.position // Element position on screen
            - (_root.worldBound.position - _startMousePosition); // Adjust start position to mouse position
            _root.CapturePointer(evt.pointerId);
        }
        evt.StopPropagation();
    }

    public void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (!enabled)
        {
            return;
        }

        if (_root.HasPointerCapture(evt.pointerId))
        {
            dragged = true;
            Vector2 diff = _startMousePosition
            - new Vector2(evt.position.x, evt.position.y) // Apply mouse movement delta
            + new Vector2(_root.layout.width / 2f, _root.layout.height / 2f); // center the slot
            DragFlotatingSlot(diff);
        }
        evt.StopPropagation();
    }

    private void UpdateFlotatingSlotAppearance()
    {
        L2SlotManager.Instance.SetDraggedSlot(_slot);
    }

    private void DragFlotatingSlot(Vector2 diff)
    {
        float right = Screen.width - _startPosition.x - _root.layout.width + diff.x;
        float bottom = Screen.height - _startPosition.y - _root.layout.height + diff.y;
        L2SlotManager.Instance.DragSlot(right, bottom);
    }

    public void PointerUpHandler(PointerUpEvent evt)
    {
        if (_root.HasPointerCapture(evt.pointerId))
        {
            _root.ReleasePointer(evt.pointerId);
            if (dragged)
            {
                L2SlotManager.Instance.ReleaseDrag();
            }
        }
        evt.StopPropagation();
    }
}