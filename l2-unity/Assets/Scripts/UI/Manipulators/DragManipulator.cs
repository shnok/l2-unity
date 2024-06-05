using UnityEngine;
using UnityEngine.UIElements;

public class DragManipulator : PointerManipulator {
    private VisualElement _root;
    private Vector2 _startMousePosition;
    private Vector2 _startPosition;

    public DragManipulator(VisualElement target, VisualElement root) {
        this.target = target;
        this._root = root;
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
            _startPosition = _root.layout.position; //+ target.layout.position;
            target.CapturePointer(evt.pointerId);   
        }
        evt.StopPropagation();
    }

    private void PointerMoveHandler(PointerMoveEvent evt) {
        if(target.HasPointerCapture(evt.pointerId)) {
            Vector2 diff = _startMousePosition - new Vector2(evt.position.x, evt.position.y);
            _root.style.left = _startPosition.x - diff.x;
            _root.style.top = _startPosition.y - diff.y;
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
