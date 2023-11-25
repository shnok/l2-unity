using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDropManipulator : PointerManipulator {
    private VisualElement target;
    private VisualElement root;
    private Vector2 startMousePosition;
    private Vector2 startPosition;

    public DragAndDropManipulator(VisualElement target, VisualElement root) {
        this.target = target;
        this.root = root;
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
            startMousePosition = evt.position;
            startPosition = root.layout.position;
            target.CapturePointer(evt.pointerId);   
        }
        evt.StopPropagation();
    }

    private void PointerMoveHandler(PointerMoveEvent evt) {
        if(target.HasPointerCapture(evt.pointerId)) {
            Vector2 diff = startMousePosition - new Vector2(evt.position.x, evt.position.y);
            root.style.left = startPosition.x - diff.x;
            root.style.top = startPosition.y - diff.y;
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
