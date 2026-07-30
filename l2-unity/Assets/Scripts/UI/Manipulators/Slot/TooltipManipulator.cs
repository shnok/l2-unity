using UnityEngine.UIElements;

public class TooltipManipulator : Manipulator
{

    private object _value;
    private readonly L2Slot.SlotType _type;
    private bool _pointerOver;

    public TooltipManipulator(VisualElement target, L2Slot.SlotType type, object value)
    {
        this.target = target;
        _type = type;
        _value = value;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerEnterEvent>(PointerInHandler, TrickleDown.TrickleDown);
        target.RegisterCallback<PointerOutEvent>(PointerOutHandler, TrickleDown.TrickleDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerEnterEvent>(PointerInHandler, TrickleDown.TrickleDown);
        target.UnregisterCallback<PointerOutEvent>(PointerOutHandler, TrickleDown.TrickleDown);
    }

    private void PointerInHandler(PointerEnterEvent evt)
    {
        if (!_pointerOver)
        {
            L2ToolTip.Instance.UpdateTooltip(_type, _value, target);
        }
        _pointerOver = true;
    }

    private void PointerOverHandler(MouseOverEvent evt)
    {
        _pointerOver = true;
    }

    private void PointerOutHandler(PointerOutEvent evt)
    {
        _pointerOver = false;
        if (_value is not null) L2ToolTip.Instance.HideWindow(target);
    }

    public void SetValue(string value)
    {
        _value = value;
    }
    
    public void SetValue(SkillWindowInfo value)
    {
        _value = value;
    }

    public void SetValue(Buff value)
    {
        _value = value;
    }

    public void Clear()
    {
        if (_pointerOver)
        {
            L2ToolTip.Instance.HideWindow(target);
        }
    }
}
