using UnityEngine.UIElements;

public class TooltipManipulator : PointerManipulator
{

    private string _text;
    private bool _pointerOver;

    public TooltipManipulator(VisualElement target, string text)
    {
        this.target = target;
        _text = text;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerEnterEvent>(PointerInHandler);
        target.RegisterCallback<MouseOverEvent>(PointerOverHandler);
        target.RegisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerEnterEvent>(PointerInHandler);
        target.UnregisterCallback<MouseOverEvent>(PointerOverHandler);
        target.UnregisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    private void PointerInHandler(PointerEnterEvent evt)
    {
        if (_text.Length > 0)
            L2ToolTip.Instance.UpdateTooltip(_text, target);
    }

    private void PointerOverHandler(MouseOverEvent evt)
    {
        _pointerOver = true;
    }

    private void PointerOutHandler(PointerOutEvent evt)
    {
        _pointerOver = false;
        if (_text.Length > 0)
            L2ToolTip.Instance.HideWindow(target);
    }

    public void SetText(string text)
    {
        _text = text;
    }

    public void Clear()
    {
        if (_pointerOver)
        {
            L2ToolTip.Instance.HideWindow(target);
        }
    }
}
