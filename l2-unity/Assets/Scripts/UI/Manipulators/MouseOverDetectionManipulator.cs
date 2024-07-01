using UnityEngine.UIElements;
using UnityEngine;

public class MouseOverDetectionManipulator : PointerManipulator {
    private bool _enabled;
    private bool _overThisManipulator = false;

    public MouseOverDetectionManipulator(VisualElement target) {
        _enabled = true;
        this.target = target;
    }

    public void Enable() {
        _enabled = true;
    }

    public void Disable() {
        _enabled = false;
        if(_overThisManipulator) {
            L2GameUI.Instance.MouseOverUI = false;
        }
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
        if (_enabled) {
            L2GameUI.Instance.MouseOverUI = true;
            _overThisManipulator = true;
        }
    }

    private void PointerOverHandler(PointerOverEvent evt) {
        if (_enabled) {
            L2GameUI.Instance.MouseOverUI = true;
            _overThisManipulator = true;
        }
    }

    private void PointerOutHandler(PointerOutEvent evt) {
        if (_enabled) {
            L2GameUI.Instance.MouseOverUI = false;
            _overThisManipulator = false;
        }
    }
}
