using UnityEngine.UIElements;
using UnityEngine;

public class MouseOverDetectionManipulator : PointerManipulator {
    private VisualElement target;

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
        Debug.Log("Mouse enter " + target.name);
        L2GameUI.GetInstance().mouseOverUI = true;
    }

    private void PointerOverHandler(PointerOverEvent evt) {
        L2GameUI.GetInstance().mouseOverUI = true;
    }

    private void PointerOutHandler(PointerOutEvent evt) {
        Debug.Log("Mouse exit " + target.name);
        L2GameUI.GetInstance().mouseOverUI = false;
    }
}
