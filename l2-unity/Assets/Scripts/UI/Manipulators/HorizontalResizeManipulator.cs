using UnityEngine;
using UnityEngine.UIElements;

public class HorizontalResizeManipulator : PointerManipulator {
    private VisualElement _root;
    private Vector2 _startMousePosition;
    private float _originalWidth;
    private float _minWidth;
    private float _maxWidth;

    public HorizontalResizeManipulator(VisualElement target, VisualElement root, float minWidth, float maxWidth) {
        this.target = target;
        _root = root;
        _minWidth = minWidth;
        _maxWidth = maxWidth;
    }

    protected override void RegisterCallbacksOnTarget() {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
    }

    private void PointerDownHandler(PointerDownEvent evt) {
        if(evt.button == 0) {
            _startMousePosition = evt.position;

            if(_root.resolvedStyle.width != 0) {
                _originalWidth = Mathf.Clamp(_root.resolvedStyle.width, _minWidth, _maxWidth);
            }
            target.CapturePointer(evt.pointerId);   
        }
        evt.StopPropagation();
    }

    private void PointerMoveHandler(PointerMoveEvent evt) {
        if(target.HasPointerCapture(evt.pointerId)) {
            Vector2 diff = _startMousePosition - new Vector2(evt.position.x, evt.position.y);
            _root.style.width = Mathf.Clamp(_originalWidth - diff.x, _minWidth, _maxWidth);
        }
        evt.StopPropagation();
    }

    private void PointerUpHandler(PointerUpEvent evt) {
        if(target.HasPointerCapture(evt.pointerId)) {
            target.ReleasePointer(evt.pointerId);
        }
        evt.StopPropagation();
    }
}
