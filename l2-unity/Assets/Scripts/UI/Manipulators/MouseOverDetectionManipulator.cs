using UnityEngine.UIElements;
using UnityEngine;

public class MouseOverDetectionManipulator : PointerManipulator {

    public MouseOverDetectionManipulator(VisualElement target) {
        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget() {
        target.RegisterCallback<PointerEnterEvent>(PointerEnterHandler);
        target.RegisterCallback<PointerOverEvent>(PointerOverHandler);
        target.RegisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<PointerEnterEvent>(PointerEnterHandler);
        target.UnregisterCallback<PointerOverEvent>(PointerOverHandler);
        target.UnregisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    private void PointerEnterHandler(PointerEnterEvent evt) {
        L2GameUI.Instance.MouseOverUI = true;
    }

    private void PointerOverHandler(PointerOverEvent evt) {
        L2GameUI.Instance.MouseOverUI = true;
    }

    private void PointerOutHandler(PointerOutEvent evt) {
        L2GameUI.Instance.MouseOverUI = false;
    }
}
