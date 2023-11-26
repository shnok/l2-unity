using UnityEngine;
using UnityEngine.UIElements;

public class HorizontalResizeManipulator : PointerManipulator {
    private VisualElement root;
    private Vector2 startMousePosition;
    private float originalWidth;
    private float minWidth;
    private float maxWidth;

    public HorizontalResizeManipulator(VisualElement target, VisualElement root, float originalWidth, float minWidth, float maxWidth) {
        this.target = target;
        this.root = root;
        this.originalWidth = originalWidth;
        this.minWidth = minWidth;
        this.maxWidth = maxWidth;
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

            if(root.style.width.value.value != 0) {
                originalWidth = Mathf.Clamp(root.style.width.value.value, minWidth, maxWidth);
            }
            target.CapturePointer(evt.pointerId);   
        }
        evt.StopPropagation();
    }

    private void PointerMoveHandler(PointerMoveEvent evt) {
        if(target.HasPointerCapture(evt.pointerId)) {
            Vector2 diff = startMousePosition - new Vector2(evt.position.x, evt.position.y);
            root.style.width = Mathf.Clamp(originalWidth - diff.x, minWidth, maxWidth);
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
