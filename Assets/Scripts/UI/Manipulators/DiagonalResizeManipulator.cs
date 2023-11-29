using UnityEngine;
using UnityEngine.UIElements;

public class DiagonalResizeManipulator : PointerManipulator {
    private VisualElement root;
    private Vector2 startMousePosition;
    private float originalWidth;
    private float originalHeight;
    private float minWidth;
    private float maxWidth;
    private float minHeight;
    private float maxHeight;

    public DiagonalResizeManipulator(VisualElement target, VisualElement root, float minWidth, float maxWidth, float minHeight, float maxHeight) {
        this.target = target;
        this.root = root;
        this.minWidth = minWidth;
        this.maxWidth = maxWidth;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
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

            if(root.resolvedStyle.width != 0) {
                originalWidth = Mathf.Clamp(root.resolvedStyle.width, minWidth, maxWidth);
                originalHeight = Mathf.Clamp(root.resolvedStyle.height, minHeight, maxHeight);
            }
            target.CapturePointer(evt.pointerId);   
        }
        evt.StopPropagation();
    }

    private void PointerMoveHandler(PointerMoveEvent evt) {
        if(target.HasPointerCapture(evt.pointerId)) {
            Vector2 diffx = startMousePosition - new Vector2(evt.position.x, evt.position.y);
            Vector2 diffy = new Vector2(evt.position.x, evt.position.y) - startMousePosition;
            root.style.width = Mathf.Clamp(originalWidth - diffx.x, minWidth, maxWidth);
            root.style.height = Mathf.Clamp(originalHeight - diffy.y, minHeight, maxHeight);
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
