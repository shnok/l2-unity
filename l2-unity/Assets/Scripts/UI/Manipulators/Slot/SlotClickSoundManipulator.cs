using UnityEngine.UIElements;

public class SlotClickSoundManipulator : PointerManipulator {
    private VisualElement _root;
    public SlotClickSoundManipulator(VisualElement target) {
        _root = target;
        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget() {
        _root.RegisterCallback<PointerDownEvent>(MouseDownHandler, TrickleDown.TrickleDown);
    }

    protected override void UnregisterCallbacksFromTarget() {
        _root.UnregisterCallback<PointerDownEvent>(MouseDownHandler);
    }

    private void MouseDownHandler(PointerDownEvent evt) {
        AudioManager.Instance.PlayUISound("click_03");
    }
}