using UnityEngine.UIElements;

public class SlotClickSoundManipulator : PointerManipulator {

    public SlotClickSoundManipulator(VisualElement target) {
        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget() {
        target.RegisterCallback<PointerDownEvent>(MouseDownHandler, TrickleDown.TrickleDown);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<PointerDownEvent>(MouseDownHandler);
    }

    private void MouseDownHandler(PointerDownEvent evt) {
        AudioManager.Instance.PlayUISound("click_03");
    }
}