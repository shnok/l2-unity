using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonClickSoundManipulator : PointerManipulator {

    public ButtonClickSoundManipulator(VisualElement target) {
        this.target = target;
    }

    protected override void RegisterCallbacksOnTarget() {
        target.RegisterCallback<MouseDownEvent>(MouseDownHandler, TrickleDown.TrickleDown);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<MouseDownEvent>(MouseDownHandler);
    }

    private void MouseDownHandler(MouseDownEvent evt) {
        AudioManager.Instance.PlayUISound("click_01");
    }
}