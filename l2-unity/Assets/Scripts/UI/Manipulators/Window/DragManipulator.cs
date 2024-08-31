using UnityEngine;
using UnityEngine.UIElements;

public class DragManipulator : PointerManipulator
{
    private VisualElement _root;
    private Vector2 _startMousePosition;
    private Vector2 _startPosition;

    private bool _rightAnchor = true;
    private bool _bottomAnchor = true;
    public bool dragged = false;

    public DragManipulator(VisualElement target, VisualElement root)
    {
        this.target = target;
        this._root = root;
        UpdateAnchorType();
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
        if (evt.button == 0)
        {
            dragged = false;
            _startMousePosition = evt.position;
            _startPosition = _root.layout.position;// + target.layout.position;
            target.CapturePointer(evt.pointerId);
        }
    }

    public void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (target.HasPointerCapture(evt.pointerId))
        {
            dragged = true;
            Vector2 diff = _startMousePosition - new Vector2(evt.position.x, evt.position.y);
            DragWindow(diff);
        }
        evt.StopPropagation();
    }

    public void PointerUpHandler(PointerUpEvent evt)
    {
        if (target.HasPointerCapture(evt.pointerId))
        {
            target.ReleasePointer(evt.pointerId);
        }

        UpdateAnchorType();
    }

    // Apply drag
    private void DragWindow(Vector2 diff)
    {
        UpdateAnchorType();

        if (_rightAnchor)
        {
            _root.style.right = Screen.width - _startPosition.x - _root.layout.width + diff.x;
            _root.style.left = StyleKeyword.Null;
        }
        else
        {
            _root.style.left = _startPosition.x - diff.x;
            _root.style.right = StyleKeyword.Null;
        }

        if (_bottomAnchor)
        {
            _root.style.bottom = Screen.height - _startPosition.y - _root.layout.height + diff.y;
            _root.style.top = StyleKeyword.Null;
        }
        else
        {
            _root.style.top = _startPosition.y - diff.y;
            _root.style.bottom = StyleKeyword.Null;
        }
    }

    // Update the anchor based on the window position ratio
    private void UpdateAnchorType()
    {
        _rightAnchor = _root.layout.position.x + (_root.layout.width / 2f) >= Screen.width / 2f;
        _bottomAnchor = _root.layout.position.y + (_root.layout.height / 2f) >= Screen.height / 2f;
    }
}
