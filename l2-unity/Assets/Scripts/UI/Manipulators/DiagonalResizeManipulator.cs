using UnityEngine;
using UnityEngine.UIElements;

public class DiagonalResizeManipulator : PointerManipulator {
    private VisualElement _root;
    private Vector2 _startMousePosition;
    private float _originalWidth;
    private float _originalHeight;
    private float _minWidth;
    private float _maxWidth;
    private float _minHeight;
    private float _maxHeight;
    private float _snapSize;
    private float _snapOffset;
    private bool _snap = false;

    public DiagonalResizeManipulator(
        VisualElement target, 
        VisualElement root, 
        float minWidth, 
        float maxWidth, 
        float minHeight, 
        float maxHeight, 
        float snapSize, 
        float snapOffset) {
        this.target = target;
        _root = root;
        _minWidth = minWidth;
        _maxWidth = maxWidth;
        _minHeight = minHeight;
        _maxHeight = maxHeight;
        _snapSize = snapSize;
        _snapOffset = snapOffset;
        _snap = true;
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
                _originalHeight = Mathf.Clamp(_root.resolvedStyle.height, _minHeight, _maxHeight);
            }

            target.CapturePointer(evt.pointerId);   
        }
        evt.StopPropagation();
    }

    private void PointerMoveHandler(PointerMoveEvent evt) {
        if(target.HasPointerCapture(evt.pointerId)) {
            Vector2 diffx = _startMousePosition - new Vector2(evt.position.x, evt.position.y);
            Vector2 diffy = new Vector2(evt.position.x, evt.position.y) - _startMousePosition;
            _root.style.width = Mathf.Clamp(_originalWidth - diffx.x, _minWidth, _maxWidth);

            float yDiff = Mathf.Clamp(_originalHeight - diffy.y, _minHeight, _maxHeight);
            if(_snap) {
                float snappedY = yDiff - yDiff % _snapSize;
                _root.style.height = snappedY + _snapOffset;
            } else {
                _root.style.width = yDiff;
            }
        }
        evt.StopPropagation();
    }

    private void PointerUpHandler(PointerUpEvent evt) {
        if(target.HasPointerCapture(evt.pointerId)) {
            target.ReleasePointer(evt.pointerId);
        }
        evt.StopPropagation();
    }

    public void SnapSize() {
        /* Initial snap */
        float snappedY = _minHeight - _minHeight % _snapSize;
        _root.style.height = snappedY + _snapOffset;
    }
}
